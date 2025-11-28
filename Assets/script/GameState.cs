using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    // 1. 流程管理功能：反派先抽卡 → 反派放卡 →主角抽卡 →主角抽完卡后点确认 → 主角放卡的同时打出伤害/护甲/技能 →点击结束回合 →反派行动
    // 0：游戏初始状态
    // 1：敌人抽卡阶段
    // 2：敌人行动阶段
    // 3：主角抽卡阶段
    // 4：主角行动阶段
    // 5: 敌人攻击阶段
    // 11: 拓印阶段
    // 99：GameEnd
    public int GameStage = 0;
    // 游戏双方
    public GameObject PlayerInstance;
    public GameObject EnemyInstance;

    [Header("拓印面板")]
    public GameObject TayinPanel;

    [Header("拓印池")]
    public TayinLibrary tayinLibrary;
    [Header("玩家牌库")]
    public CardLibary cardLibary;

    // Start is called before the first frame update
    void Start()
    {
        if(this.GameStage == 0){
            this.GameStage = 1;
        }
        PlayerInstance = GameObject.Find("Player");
        EnemyInstance = GameObject.Find("Enemy");
        tayinLibrary = new TayinLibrary();
        tayinLibrary.LoadLibrary();
        cardLibary = new CardLibary();
        cardLibary.LoadLibrary();
    }

    // Update is called once per frame
    void Update()
    {
        // 如敌人死亡进入reward界面
        if (EnemyInstance.GetComponent<BaseEntity>().isDead())
        {
            Debug.Log("Enemy is dead");
            this.GameStage = 11;
            this.TayinPanel.SetActive(true);
        }
        // 如果角色死亡进入gameover界面
        if (PlayerInstance.GetComponent<BaseEntity>().isDead())
        {
            Debug.Log("Player is dead");
            this.GameStage = 99;
        }
    }
}
