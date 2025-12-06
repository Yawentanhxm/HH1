using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EnemyAI : BaseEntity
{
    // 游戏内一些全局状态
    public GameObject gameManage;
    // 状态
    // public Object status;
    [Header("弃牌堆")]
    public List<CardData> DiscardCard = new List<CardData>();

    [Header("思考标记位")]
    private bool isThinking = false;
    private bool isAction = false;

    // 卡片模型
    [Header("卡片模型")]
    public GameObject cardPrefab;
    public GameObject drawArea;
    public GameObject cardLibaryInstance;

    // Start is called before the first frame update
    void Start()
    {
        this.gameManage = GameObject.Find("GameManage");
        this.gameState = gameManage.GetComponent<GameState>();
        this.HPBar = this.transform.Find("hp").gameObject;
        if (this.gameState != null){
            Debug.Log(this.gameState.GameStage);
        }
        this.MaxHP = 20;
        this.HP = 20;
        // 初始化牌库
        this.cardLibaryUtils.LoadLibrary("data/enemy_library_data.json");
        this.cardLibary = this.cardLibaryUtils.cardList;
        this.cardLibaryData = this.cardLibaryUtils.cardList;

        attackInstance = GameObject.Find("attack");
        defInstance = GameObject.Find("shield");
    }

    // Update is called once per frame
    void Update()
    {
        switch (this.gameState.GameStage)
        {
            case GameStageType.GameStart:
                // 抽牌
                if (this.firstDraw)
                {
                    FirstDraw();
                }
                break;
            case GameStageType.EnemyDraw:
                if(!this.drawEnd) {
                    // 分帧实现异步思考
                    Evaluate();
                }
                this.gameState.DrawDone();
                break;
            case GameStageType.EnemyAction:
                if(!this.actionEnd) {
                    // 分帧实现异步思考
                    action();
                }
                this.gameState.ActionDone();
                break;
            case GameStageType.EnemyAttack:
                // 执行
                execute();
                break;
        }
    }

    // 抽卡逻辑
    void draw()
    {
        this.handCard.Add(cardLibary[cardLibary.Count - 1]);
        cardLibary.RemoveAt(cardLibary.Count - 1);
        CardData newCardData = this.handCard[handCard.Count - 1];
        // 实例化模型，并且添加到手牌区域
        // 实例化到指定父物体，不保持世界坐标（本地坐标会重置）
        GameObject newCard = Instantiate(cardPrefab, drawArea.transform);
        // 设置卡片属性
        Card cardScript = newCard.GetComponent<Card>();
        cardScript.ownerEntity = gameState.EnemyInstance.GetComponent<BaseEntity>();;
        cardScript.enemyEntity = gameState.PlayerInstance.GetComponent<BaseEntity>();;
        cardScript.cardData = newCardData;
        cardScript.isDraggable = false;

        // 保存目标位置
        Vector3 targetPosition = new Vector3(-50 - handCard.Count * 225, 0, 0);

        // 设置初始位置为牌库位置（可以根据实际牌库位置调整）
        newCard.transform.localPosition = cardLibaryInstance.transform.localPosition;
        
        // 添加动画协程
        StartCoroutine(MoveCardToHand(newCard, targetPosition));

        Transform left = newCard.transform.Find("CardBase/Content/left");
        if (left != null) {
            Text textComponent = left.GetComponent<Text>();

            if (textComponent!=null){
                textComponent.text = newCardData.cardName;
                textComponent.fontSize  = 40;
            }
        }

        
        Transform right = newCard.transform.Find("CardBase/Content/right");
        if (right != null) {
            Text textComponent = right.GetComponent<Text>();

            if (textComponent!=null){
                textComponent.text = newCardData.cardName;
                textComponent.fontSize  = 40;
            }
        }

        Debug.Log("抽到了"+newCardData.cardName + newCardData.cardNum + newCardData.property);
        handCardPrefab.Add(newCard);
        this.handPoint += newCardData.cardNum;
    }

    public void FirstDraw()
    {
        // 第一帧：判断是否是该回合第一次抽卡
        if(firstDraw && !this.drawEnd)
        {
            this.draw();
            this.draw();
            firstDraw = false;
        }
    }
    private bool ShouldDraw()
    {
        // 判断不在抽卡
        if (this.handPoint + cardLibary[cardLibary.Count - 1].cardNum > 21) {
            return false;
        }
        return true;
    }
    public void Evaluate()
    {
        // 第二帧：评估抽卡带来的收益，从而决定是否抽卡
        // 直接判断下张卡会不会炸
        // 抽完卡后进入到主角抽卡阶段
        // 判断不在抽卡
        if (!this.drawEnd && ShouldDraw()) {
            this.draw();
        }else{
            this.drawEnd = true;
        }
    }
    
    
    private bool ShouldYieldFrame()
    {
        // 根据性能动态调整每帧处理的数量
        return Time.frameCount % 3 == 0;
    }


    // 行动逻辑逻辑
    public void action()
    {
        Debug.Log("敌人开始行动");
        // AI行为逻辑
        // 选取一张牌使用
        if (handCard.Count <= 0)
        {
            Debug.Log("没有牌了");
            this.gameState.SetStage(GameStageType.PlayerAction);
            this.actionEnd = true;
            return;
        }
        // 行动逻辑
        if(handCard[0].cardNum>=5){
            attackCard.Add(handCardPrefab[0]);
            Vector3 targetPosition = attackInstance.transform.position;
            // 添加动画协程
            StartCoroutine(MoveCardToWorldPosition(handCardPrefab[0], targetPosition));
        }else{
            defCard.Add(handCardPrefab[0]);
            Vector3 targetPosition = defInstance.transform.position;
            // 添加动画协程
            StartCoroutine(MoveCardToWorldPosition(handCardPrefab[0], targetPosition));
        }
        // 记录卡片放置顺序
        actionCardPrefab.Add(handCardPrefab[0]);
        handCard.RemoveAt(0);
        handCardPrefab.RemoveAt(0);
        this.gameState.SetStage(GameStageType.PlayerAction);
    }

    public void execute()
    {
        for(int i=0; i<attackCard.Count; i++){
            Debug.Log("敌人使用第" + i + "张牌");
            if(attackCard[i]) {
                attackCard[i].GetComponent<Card>().execute("attack");
            }
        }
    }
}
