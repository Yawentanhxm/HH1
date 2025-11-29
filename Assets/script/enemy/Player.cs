using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.Json;

public class Player : BaseEntity
{
    // 游戏内一些全局状态
    public GameObject gameObject;
    // 状态
    // public Object status;
    [Header("弃牌堆")]
    public List<CardData> DiscardCard = new List<CardData>();

    // 卡片模型
    public GameObject cardPrefab;
    public GameObject drawArea;
    [Header("牌库")]
    public GameObject cardLibaryInstance;

    public Button drawButton;
    public Button endButton;

    public CardData newCard(int i)
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
        return temp;
    }


    void OnButtonClicked()
    {
        // 抽卡阶段且未过载
        if(this.gameState.GameStage == 3 && overload == false) {
            draw();
            // 抽完卡进入敌人抽卡环节
            this.gameState.GameStage = 1;
        }
    }

    void OnStageEnd()
    {
        Debug.Log("Button找到，开始添加监听");
        // 抽卡阶段结束回合结束
        if(this.gameState.GameStage == 3) {
            this.gameState.GameStage = 1;
            this.drawEnd = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject = GameObject.Find("GameManage");
        this.gameState = gameObject.GetComponent<GameState>();
        if (this.gameState != null){
            Debug.Log(this.gameState.GameStage);
        }
        // init_libary();
        // 读取文件加载牌库
        this.cardLibaryUtils.LoadLibrary("data/player_library_data.json");
        this.cardLibary = this.cardLibaryUtils.cardList;
        this.cardLibaryData = this.cardLibaryUtils.cardList;
        this.HPBar = this.transform.Find("hp").gameObject;
        this.MaxHP = 100;
        this.HP = 100;
        this.firstDraw = true;
        
        Debug.Log("Button找到，开始添加监听");
        // 通过代码添加监听
        drawButton.onClick.AddListener(OnButtonClicked);
        endButton.onClick.AddListener(OnStageEnd);
        Debug.Log("AddListener完毕");
    }

    // Update is called once per frame
    void Update()
    {
        switch (this.gameState.GameStage)
        {
            case 0:
                if (this.firstDraw)
                {
                    FirstDraw();
                }
                break;
            case 3:
                if (IsOverLoad())
                {
                    this.drawEnd = true;
                    this.actionEnd = true;
                    this.gameState.GameStage = 1;
                    for (int i = 0; i < handCardPrefab.Count; i++) {
                        if (handCardPrefab[i])
                        {
                            Destroy(handCardPrefab[i]);
                        }
                    }
                }else if (this.drawEnd)
                {
                    // 抽完卡进入敌人抽卡环节
                    this.gameState.GameStage = 1;
                }
                break;
            case 4:
                // 行动阶段没结束，设置可怕可拖拽
                if (!this.actionEnd)
                {
                    for (int i = 0; i < handCardPrefab.Count; i++) {
                        if (handCardPrefab[i])
                        {
                            handCardPrefab[i].GetComponent<Card>().isDraggable = true;
                        }
                    }
                    if (handCardPrefab.Count <= 0)
                    {
                        this.actionEnd = true;
                    }
                }
                else
                {
                    this.gameState.GameStage = 2;
                }
                break;
        }
    }


    // 抽卡逻辑
    void draw()
    {
        this.handCard.Add(cardLibary[cardLibary.Count - 1]);
        CardData newCardData = this.handCard[handCard.Count - 1];
        cardLibary.RemoveAt(cardLibary.Count - 1);
        // 实例化模型，并且添加到手牌区域
        
        // 实例化到指定父物体，不保持世界坐标（本地坐标会重置）
        GameObject newCard = Instantiate(cardPrefab, drawArea.transform);
        // 设置卡片属性
        Card cardScript = newCard.GetComponent<Card>();
        cardScript.ownerEntity = gameState.PlayerInstance.GetComponent<BaseEntity>();
        cardScript.enemyEntity = gameState.EnemyInstance.GetComponent<BaseEntity>();
        cardScript.cardData = newCardData;
        cardScript.isDraggable = false;


        // 保存目标位置
        Vector3 targetPosition = new Vector3(handCard.Count * 225, 0, 0);
        // 设置初始位置为牌库位置（可以根据实际牌库位置调整）
        newCard.transform.position = cardLibaryInstance.transform.position;
        
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
        this.handPoint += newCardData.cardNum;
        Debug.Log("抽到了"+newCardData.cardName + newCardData.cardNum + newCardData.property);
        this.handCardPrefab.Add(newCard);
    }
    

    public void FirstDraw()
    {
        // 第一帧：判断是否是该回合第一次抽卡
        if(firstDraw)
        {
            this.draw();
            this.draw();
            firstDraw = false;
        }
    }

    public void Evaluate()
    {
        // 监听按钮，判断是否抽牌
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            this.draw();
            if(handPoint > 21) {
                Debug.Log("过载了");
                overload = true;
                this.actionEnd = true;
                this.drawEnd = true;
                this.gameState.GameStage = 2;
            }
        }else if(Input.GetKeyDown(KeyCode.Escape)){
            this.gameState.GameStage = 4;
        }
    }
    
    
    private bool ShouldYieldFrame()
    {
        // 根据性能动态调整每帧处理的数量
        return Time.frameCount % 3 == 0;
    }
}
