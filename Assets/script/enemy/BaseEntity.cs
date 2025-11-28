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
    


    [Header("手牌")]
    public List<CardData> handCard;
    public List<GameObject> handCardPrefab = new List<GameObject>();
    public List<GameObject> attackCard;
    public List<GameObject> defCard;
    public List<GameObject> shuCard;
    [Header("牌库原始数据")]
    public List<CardData> cardLibaryData = new List<CardData>();
    [Header("牌库")]
    public List<CardData> cardLibary = new List<CardData>();



    [Header("手牌点数和")]
    public int handPoint;
    [Header("首次抽牌标记位")]
    public bool firstDraw = true;
    [Header("过载标记位")]
    public bool overload = false;

    public void init_hand_card()
    {
        handCard = new List<CardData>();
        handCardPrefab = new List<GameObject>();
        attackCard = new List<GameObject>();
        defCard = new List<GameObject>();
        shuCard = new List<GameObject>();
        this.handPoint = 0;
        this.firstDraw = true;
        this.overload = false;
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

    public bool isDead()
    {
        return this.HP <= 0;
    }
}