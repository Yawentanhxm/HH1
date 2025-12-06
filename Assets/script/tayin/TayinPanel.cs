using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TayinPanel : MonoBehaviour
{ 
    [Header("游戏管理器")]
    public GameState gameState;
    [Header("卡片prefab")]
    public GameObject cardPrefab;
    [Header("拓印prefab")]
    public GameObject tayinPrefab;
    // Start is called before the first frame update
    [Header("拓印面板")]
    public List<List<GameObject>> cardList = new List<List<GameObject>>();
    [Header("拓印技能")]
    public List<GameObject> tayinList = new List<GameObject>();

    public List<CardData> cardLibary;
    public bool isActiveAndEnabled = false;

    void Start()
    {

    }

    void OnEnable()
    {
        this.isActiveAndEnabled = true;
        InitTayin();
        InitCard();
    }

    void InitTayin()
    {
        HashSet<int> cardNum = new HashSet<int>();
        // 随机抽出 3 张牌
        for (int i = 0; i < 3; i++)
        { 
            int index = Random.Range(0, gameState.tayinLibrary.tayinList.Count);
            if (cardNum.Contains(index))
            {
                i--;
                continue;
            }
            else
            {
                cardNum.Add(index);
            }
        }
        int[] cardNumList = cardNum.ToArray();
        Transform rootTransform = transform.Find("SelectTayin");
        // // 随机抽出 3个拓印技能
        for (int i = 0; i < 3; i++){
            // 设置相对于当前父节点相对坐标的模型
            GameObject tayin = Instantiate(tayinPrefab, rootTransform);
            TayinEntity tayinEntity = tayin.GetComponent<TayinEntity>();
            // TayinEntity temp = gameState.tayinLibrary.GetRandomTayinData();
            
            TayinEntity temp = gameState.tayinLibrary.GetTayinDataById(cardNumList[i]);
            
            tayinEntity.SetTayinData(temp);
            // 设置相对于当前父节点相对坐标的模型
            tayin.transform.localPosition = new Vector3(0, 250 - i * 250, 0);
            Debug.Log("拓印技能名称：" + tayinEntity.Name);
            tayin.GetComponentInChildren<Text>().text = tayinEntity.Name;
            tayinList.Add(tayin);
        }
    }

    void InitCard()
    {
        HashSet<int> cardNum = new HashSet<int>();
        cardLibary = gameState.cardLibary.cardList;
        // 随机抽出 9 张牌
        for (int i = 0; i < 9; i++)
        { 
            int index = Random.Range(0, cardLibary.Count);
            if (cardNum.Contains(index))
            {
                i--;
                continue;
            }
            else
            {
                cardNum.Add(index);
            }
        }
        int[] cardNumList = cardNum.ToArray();
        for (int i = 0; i < 3; i++){
            List<GameObject> temp = new List<GameObject>();
            for (int j = 0; j < 3; j++)
                {
                    CardData cardData = cardLibary[cardNumList[i*3+j]];
                    Debug.Log($"当前抽取第{cardData}张");
                    // cardLibary.RemoveAt(index);
                    Debug.Log("抽取的牌是：" + cardData.cardName);
                    // 设置相对于当前父节点相对坐标的模型
                    GameObject card = Instantiate(cardPrefab, transform.Find("Tayin"));
                    card.GetComponent<Card>().setCardData(cardData);
                    card.GetComponent<Card>().isDraggable = false;
                    // 缩放1.5倍
                    card.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    // 设置相对于当前父节点相对坐标的模型
                    card.transform.localPosition = new Vector3(350 - i * 300, 250 - j * 250, 0);
                    
                    temp.Add(card);
                }
            cardList.Add(temp);
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            for (int j = 0; j < cardList[i].Count; j++)
            {
                Destroy(cardList[i][j]);
            }
        }
        cardList.Clear();

        for (int i = 0; i < tayinList.Count; i++)
        {
            Destroy(tayinList[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 更合理的隐藏条件检查
        if (this.isActiveAndEnabled && ShouldHidePanel())
        {
            this.gameObject.SetActive(false);
            this.isActiveAndEnabled = false;
            this.gameState.SetStage(GameStageType.NextLevel);
        }
    }

    private bool ShouldHidePanel()
    {
        // 根据实际游戏逻辑判断是否应该隐藏面板
        // 例如：玩家已完成选择、回合结束等
        if (gameState.GameStage == GameStageType.Tayin)
        {
            for (int i = 0; i < tayinList.Count; i++)
            {
                if (tayinList[i])
                {
                    return false;
                }
            }
            return true;
        }
        return false; // 根据你的具体需求实现
    }
}
