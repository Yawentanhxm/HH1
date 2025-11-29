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
    // 7: 行动结束计算伤害
    // 6: 加载下一关
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
        PlayerInstance = GameObject.Find("Player");
        EnemyInstance = GameObject.Find("Enemy");
        tayinLibrary = new TayinLibrary();
        tayinLibrary.LoadLibrary();
        cardLibary = new CardLibary();
        cardLibary.LoadLibrary("data/player_library_data.json");
    }

    // Update is called once per frame
    void Update()
    {
        switch (this.GameStage)
        {
            case 0:
            if (!EnemyInstance.GetComponent<BaseEntity>().firstDraw && !PlayerInstance.GetComponent<BaseEntity>().firstDraw)
            {
                this.GameStage = 1;
            }
                break;
            case 6:
                // 新关卡
                EnemyInstance.GetComponent<BaseEntity>().addHP(EnemyInstance.GetComponent<BaseEntity>().MaxHP);
                this.GameStage = 1;
                break;
            case 1: case 3:
                // 双方抽牌结束
                if(EnemyInstance.GetComponent<EnemyAI>().drawEnd && PlayerInstance.GetComponent<BaseEntity>().drawEnd)
                {
                    this.GameStage = 2;
                }
                break;
            case 2:case 4:
                // 双方行动结束，计算伤害
                if(EnemyInstance.GetComponent<EnemyAI>().actionEnd && PlayerInstance.GetComponent<BaseEntity>().actionEnd)
                {
                    this.GameStage = 7;
                }
                break;
            case 7:
                // 按照顺序出发卡片效果
                ActionCardExcute();
                EnemyInstance.GetComponent<BaseEntity>().Restart();
                PlayerInstance.GetComponent<BaseEntity>().Restart();
                this.GameStage = 0;
                break;
        }

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
    public void ActionCardExcute()
    {
        int i = 0;
        while (i < EnemyInstance.GetComponent<BaseEntity>().actionCardPrefab.Count || i < PlayerInstance.GetComponent<BaseEntity>().actionCardPrefab.Count)
        {
            if (EnemyInstance.GetComponent<BaseEntity>().actionCardPrefab.Count > i)
            {
                EnemyInstance.GetComponent<BaseEntity>().actionCardPrefab[i].GetComponent<Card>().actionCard();
            }
            if (PlayerInstance.GetComponent<BaseEntity>().actionCardPrefab.Count > i)
            {
                PlayerInstance.GetComponent<BaseEntity>().actionCardPrefab[i].GetComponent<Card>().actionCard();
            }
            i++;
        }
    }
}
