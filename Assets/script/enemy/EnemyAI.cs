using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EnemyAI : BaseEntity
{
    // 游戏内一些全局状态
    public GameObject gameManage;
    public GameState gameState;
    // 状态
    // public Object status;
    [Header("弃牌堆")]
    public List<CardData> DiscardCard = new List<CardData>();

    [Header("思考标记位")]
    private bool isThinking = false;
    private bool isAction = false;

    // 卡片模型
    public GameObject cardPrefab;
    public GameObject drawArea;


    void init_libary()
    {
        // todo 加大小王
        for(int i=0; i<52; i++)
        {
            CardData temp = new CardData();
            temp.cardName = ""+((i%13)+1);
            temp.cardNum = (i%13)+1;
            // 增加属性
            temp.property = i/13+1;
            if (temp.cardNum >= 10) {
                if(temp.cardNum == 11){
                    temp.cardName = "J";
                }
                if(temp.cardNum == 12){
                    temp.cardName = "Q";
                }
                if(temp.cardNum == 13){
                    temp.cardName = "K";
                }
                temp.cardNum = 10;
            }
            cardLibary.Add(temp);
        }
        System.Random rnd = new System.Random();
        cardLibary.Sort((a, b) => rnd.Next(-1, 2));
        Debug.Log("牌库初始化完毕");
    }

    // Start is called before the first frame update
    void Start()
    {
        this.gameState = gameManage.GetComponent<GameState>();
        this.HPBar = this.transform.Find("hp").gameObject;
        if (this.gameState != null){
            Debug.Log(this.gameState.GameStage);
        }
        // 初始化牌库
        init_libary();
        this.MaxHP = 20;
        this.HP = 20;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(this.gameState.GameStage);
        if(this.gameState.GameStage == 1) {
            // 分帧实现异步思考
            StartCoroutine(ThinkAndDecideCoroutine());
        }else if(this.gameState.GameStage == 2 && !isAction){
            StartCoroutine(action());
        }else if(this.gameState.GameStage == 5){
            StartCoroutine(execute());
            IsActionStageEnd();
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
        // 设置相对于父物体的本地位置
        newCard.transform.localPosition = new Vector3(-50 - handCard.Count * 150, 0, 0);
        Transform left = newCard.transform.Find("CardBase/Content/left");
        if (left != null) {
            Text textComponent = left.GetComponent<Text>();

            if (textComponent!=null){
                textComponent.text = newCardData.cardName;
                textComponent.fontSize  = 14;
            }
        }

        
        Transform right = newCard.transform.Find("CardBase/Content/right");
        if (right != null) {
            Text textComponent = right.GetComponent<Text>();

            if (textComponent!=null){
                textComponent.text = newCardData.cardName;
                textComponent.fontSize  = 14;
            }
        }

        Debug.Log("抽到了"+newCardData.cardName + newCardData.cardNum + newCardData.property);
        handCardPrefab.Add(newCard);
        this.handPoint += newCardData.cardNum;
        // 过载并进下个阶段
        if(this.handPoint > 21) {
            overload = true;
            this.gameState.GameStage = 4;
        }
    }

    IEnumerator ThinkAndDecideCoroutine()
    {
        // isThinking = true;
        
        // 分帧处理初始抽牌
        yield return StartCoroutine(FirstDraw());
        // 分帧处理AI思考
        yield return StartCoroutine(Evaluate());
        
        // 思考结束开始运行
        // isThinking = false;
        // yield return null;
    }

    IEnumerator FirstDraw()
    {
        // 第一帧：判断是否是该回合第一次抽卡
        if(firstDraw)
        {
            this.draw();
            this.draw();
            firstDraw = false;
        }
        yield return null;
    }

    IEnumerator Evaluate()
    {
        // 第二帧：评估抽卡带来的收益，从而决定是否抽卡
        // 直接判断下张卡会不会炸
        Debug.Log("handPoint" + this.handPoint);
        Debug.Log("next card" + cardLibary[cardLibary.Count - 1].cardNum);
        if (this.handPoint + cardLibary[cardLibary.Count - 1].cardNum > 21) {
            this.gameState.GameStage = 2;
            this.firstDraw = true;
        }else{
            this.draw();
        }
        yield return null;
    }
    
    
    private bool ShouldYieldFrame()
    {
        // 根据性能动态调整每帧处理的数量
        return Time.frameCount % 3 == 0;
    }


    // 行动逻辑逻辑
    IEnumerator action()
    {
        isAction = true;
        Debug.Log("敌人开始行动");
        // >=5攻击，<5防御
        for(int i=0; i<handCard.Count; i++){
            Debug.Log("敌人使用第" + i + "张牌");
            if(handCard[i].cardNum>=5){
                attackCard.Add(handCardPrefab[i]);
            }else{
                defCard.Add(handCardPrefab[i]);
                if(handCardPrefab[i])
                {
                    handCardPrefab[i].GetComponent<Card>().execute("shield");
                }
            }
            yield return null;
        }

        Debug.Log("hand card Count" + handCard.Count);
        Debug.Log("attackCard card Count" + attackCard.Count);
        Debug.Log("defCard card Count" + defCard.Count);
        this.gameState.GameStage = 3;
        isAction = false;
    }

    void IsActionStageEnd()
    {
        // 手牌都Miss结束阶段
        for (int i = 0; i < handCardPrefab.Count; i++) {
            if (handCardPrefab[i] != null) {
                return;
            }
        }
        this.gameState.GameStage = 1;
        this.init_hand_card();
    }

    IEnumerator execute()
    {
        for(int i=0; i<attackCard.Count; i++){
            Debug.Log("敌人使用第" + i + "张牌");
            if(attackCard[i]) {
                attackCard[i].GetComponent<Card>().execute("attack");
            }
        }
        yield return null;
    }
}
