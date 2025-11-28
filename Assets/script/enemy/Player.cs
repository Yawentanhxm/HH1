using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.Json;

public class Player : BaseEntity
{
    // 游戏内一些全局状态
    public GameObject gameObject;
    public GameState gameState;
    // 状态
    // public Object status;
    [Header("弃牌堆")]
    public List<CardData> DiscardCard = new List<CardData>();

    // 卡片模型
    public GameObject cardPrefab;
    public GameObject drawArea;

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

    void init_libary()
    {
        // todo 加大小王
        for(int i=0; i<52; i++)
        {
            CardData temp = newCard(i);
            CardData temp2 = newCard(i);
            cardLibary.Add(temp);
            cardLibaryData.Add(temp2);
        }
        System.Random rnd = new System.Random();
        cardLibary.Sort((a, b) => rnd.Next(-1, 2));
        Debug.Log("牌库初始化完毕");
        
    }

    
    void OnButtonClicked()
    {
        // 抽卡阶段且未过载
        if(this.gameState.GameStage == 3 && overload == false) {
            draw();
        }
    }

    void OnStageEnd()
    {
        Debug.Log("Button找到，开始添加监听");
        // 抽卡阶段结束回合结束
        if(this.gameState.GameStage == 3) {
            this.gameState.GameStage = 4;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.gameState = gameObject.GetComponent<GameState>();
        if (this.gameState != null){
            Debug.Log(this.gameState.GameStage);
        }
        // 初始化牌库
        init_libary();
        this.HPBar = this.transform.Find("hp").gameObject;
        this.MaxHP = 100;
        this.HP = 100;
        
        Debug.Log("Button找到，开始添加监听");
        // 通过代码添加监听
        drawButton.onClick.AddListener(OnButtonClicked);
        endButton.onClick.AddListener(OnStageEnd);
        Debug.Log("AddListener完毕");
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(this.gameState.GameStage);
        if(this.gameState.GameStage == 3) {
            StartCoroutine(ThinkAndDecideCoroutine());
        }else if(this.gameState.GameStage == 4){
            // 设置手牌可拖拽
            for (int i = 0; i < handCardPrefab.Count; i++) {
                if (handCardPrefab[i])
                {
                    handCardPrefab[i].GetComponent<Card>().isDraggable = true;
                }
            }
            StartCoroutine(action());
            IsActionStageEnd();
        }
        
    }

    void IsActionStageEnd()
    {
        // 手牌都Miss结束阶段
        for (int i = 0; i < handCardPrefab.Count; i++) {
            if (handCardPrefab[i] != null) {
                return;
            }
        }
        this.gameState.GameStage = 5;
        this.init_hand_card();
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
        // 设置相对于父物体的本地位置
        newCard.transform.localPosition = new Vector3(50 + handCard.Count * 150, 0, 0);
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
        this.handPoint += newCardData.cardNum;
        Debug.Log("抽到了"+newCardData.cardName + newCardData.cardNum + newCardData.property);
        this.handCardPrefab.Add(newCard);
        // 过载并进下个阶段
        if(this.handPoint > 21) {
            overload = true;
            this.gameState.GameStage = 4;
        }
    }

    IEnumerator ThinkAndDecideCoroutine()
    {
        // 分帧处理初始抽牌
        yield return StartCoroutine(FirstDraw());
        yield return StartCoroutine(Evaluate());
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
        // 监听按钮，判断是否抽牌
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            this.draw();
            if(handPoint > 21) {
                Debug.Log("过载了");
                overload = true;
                this.gameState.GameStage = 4;
            }
        }else if(Input.GetKeyDown(KeyCode.Escape)){
            this.gameState.GameStage = 4;
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
        // todo 监听拖拽动作
        yield return null;
    }
}
