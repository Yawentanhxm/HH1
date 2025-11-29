using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseEntity : MonoBehaviour
{
    // 基础属性
    
    public int MaxHP;
    public int HP;
    public int LP;
    public int Shied;
    [Header("Buff列表")]
    public List<GameObject> BuffList;
    [Header("状态列表")]
    public List<GameObject> StatusList;

    public GameObject HPBar;

    public GameState gameState;

    [Header("手牌")]
    public List<CardData> handCard;
    public List<GameObject> handCardPrefab = new List<GameObject>();
    
    [Header("卡片放置顺序")]
    public List<GameObject> actionCardPrefab= new List<GameObject>();
    public List<GameObject> attackCard;
    public List<GameObject> defCard;
    public List<GameObject> shuCard;
    public GameObject attackInstance;
    public GameObject defInstance;

    [Header("牌库原始数据")]
    public List<CardData> cardLibaryData = new List<CardData>();
    [Header("牌库")]
    public List<CardData> cardLibary = new List<CardData>();
    public CardLibary cardLibaryUtils = new CardLibary();



    [Header("手牌点数和")]
    public int handPoint;
    [Header("首次抽牌标记位")]
    public bool firstDraw = true;
    [Header("过载标记位")]
    public bool overload = false;

    public bool drawEnd = false;
    public bool actionEnd = false;


    public void Restart()
    {
        this.firstDraw = true;
        this.drawEnd = false;
        this.actionEnd = false;
        this.Shied = 0;
        this.handPoint = 0;
        handCard = new List<CardData>();
        handCardPrefab = new List<GameObject>();
        attackCard = new List<GameObject>();
        defCard = new List<GameObject>();
        shuCard = new List<GameObject>();
        actionCardPrefab = new List<GameObject>();
    }

    public bool IsOverLoad()
    {
        this.overload = this.handPoint > 21;
        return this.overload;
    }

    public void updateHpBar()
    {
        HPBar.GetComponent<Image>().fillAmount = (float)this.HP / (float)this.MaxHP;
    }

    public void addHP(int num)
    {
        if (this.MaxHP < this.HP + num)  {
            this.HP = this.MaxHP;
        }else{
            this.HP = this.HP + num;
        }
        this.updateHpBar();
    }
    
    public void reduceHP(int num)
    {
        num = this.reduceShied(num);
        this.HP = num>this.HP?0:this.HP-num;
        this.updateHpBar();
    }

    public void addShied(int num)
    {
        this.Shied = this.Shied + num;
    }

    // 返回剩余的伤害
    public int reduceShied(int num)
    {
        if (this.Shied <= 0) {
            return num;
        }
        this.Shied = this.Shied - num;
        if (this.Shied < 0) {
            num = -1 * this.Shied;
            this.Shied = 0;
            return num;
        }else{
            return 0;
        }
    }

    public void addBuff(int buffId, int layer)
    {
        // 添加layer层 buffId buff
        // 1. 实例化BuffPrefab，并且赋值给BuffEntity
        // BuffEntity buffEntity = Resources.Load<BuffEntity>("script/buff/buff_entity.cs");
        GameObject buffPrefab = (GameObject)Instantiate(Resources.Load("prefab/Buff"));
        BuffEntity buffEntity = buffPrefab.GetComponent<BuffEntity>();
        buffEntity.number = buffId;
        buffEntity.layer = layer;
        // 2. 添加到BuffList
        BuffList.Add(buffPrefab);
    }

    public IEnumerator MoveCardToHand(GameObject card, Vector3 targetPosition)
    {
        // 检查对象是否有效
        if (card == null)
        {
            Debug.LogWarning("Card is null, stopping coroutine");
            yield break;
        }
        Vector3 startPosition = card.transform.localPosition;
        float elapsedTime = 0f;
        float moveDuration = 0.5f; // 动画持续时间（秒）
        
        while (elapsedTime < moveDuration)
        {
            // 关键：在每次循环中都要检查card是否仍然存在
            if (card == null)
            {
                Debug.LogWarning("Card was destroyed during movement");
                yield break;
            }
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / moveDuration);
            // 使用平滑插值使动画更自然
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            card.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            yield return null;
        }
        
        // 最后再次检查对象是否存在再设置最终位置
        if (card != null)
        {
            card.transform.localPosition = targetPosition;
        }
    }

    public IEnumerator MoveCardToWorldPosition(GameObject card, Vector3 targetPosition)
    {
        // 检查对象是否有效
        if (card == null)
        {
            Debug.LogWarning("Card is null, stopping coroutine");
            yield break;
        }

        Vector3 startPosition = card.transform.position;
        float elapsedTime = 0f;
        float moveDuration = 0.5f;
        
        while (elapsedTime < moveDuration)
        {
            // 关键：在每次循环中都要检查card是否仍然存在
            if (card == null)
            {
                Debug.LogWarning("Card was destroyed during movement");
                yield break;
            }
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / moveDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            card.transform.position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            yield return null;
        }
        
        // 最后再次检查对象是否存在再设置最终位置
        if (card != null)
        {
            card.transform.position = targetPosition;
        }
    }

    public bool isDead()
    {
        return this.HP <= 0;
    }
}