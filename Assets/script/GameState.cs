using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public enum GameStageType
{
    GameStart = 0,
    EnemyDraw = 1,
    EnemyAction = 2,
    PlayerDraw = 3,
    PlayerAction = 4,
    EnemyAttack = 5,
    NextLevel = 6,
    ActionEnd = 7,
    Tayin = 11,
    GameEnd = 99,
    Wait = 999
}
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
    public GameStageType GameStage = GameStageType.GameStart;
    // 游戏双方
    public GameObject PlayerInstance;
    public GameObject EnemyInstance;

    [Header("拓印面板")]
    public GameObject TayinPanel;

    [Header("拓印池")]
    public TayinLibrary tayinLibrary;
    [Header("玩家牌库")]
    public CardLibary cardLibary;
    private Text text;
    // 等待时间（秒）
    public float stageTransitionDelay = 0.01f;
    
    // 正在等待阶段转换
    private bool isWaitingForTransition = false;
    private bool DrawFlag = false;
    private bool ActionFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInstance = GameObject.Find("Player");
        EnemyInstance = GameObject.Find("Enemy");
        tayinLibrary = new TayinLibrary();
        tayinLibrary.LoadLibrary();
        cardLibary = new CardLibary();
        cardLibary.LoadLibrary("data/player_library_data.json");
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (this.GameStage)
        {
            case GameStageType.GameStart:
                if (!EnemyInstance.GetComponent<BaseEntity>().firstDraw && !PlayerInstance.GetComponent<BaseEntity>().firstDraw)
                {
                    SetStage(GameStageType.EnemyDraw);
                }
                break;
            case GameStageType.NextLevel:
                // 新关卡
                // EnemyInstance.GetComponent<BaseEntity>().addHP(EnemyInstance.GetComponent<BaseEntity>().MaxHP);
                ReloadCurrentScene();
                SetStage(GameStageType.GameStart);
                // this.GameStage = GameStageType.GameStart;
                break;
            case GameStageType.EnemyDraw:
            case GameStageType.PlayerDraw:
                // 双方抽牌结束
                if(DrawFlag)
                {
                    if (PlayerInstance.GetComponent<BaseEntity>().drawEnd && EnemyInstance.GetComponent<BaseEntity>().drawEnd)
                    {
                        SetStage(GameStageType.EnemyAction);
                    }
                    else if(PlayerInstance.GetComponent<BaseEntity>().drawEnd)
                    {
                        SetStage(GameStageType.EnemyDraw);
                    }
                    else
                    {
                        SetStage(GameStageType.PlayerDraw);
                    }
                    this.DrawFlag = false;
                }
                break;
            case GameStageType.EnemyAction:
            case GameStageType.PlayerAction:
                if(this.ActionFlag)
                {
                    // 双方行动结束，计算伤害
                    if(EnemyInstance.GetComponent<BaseEntity>().actionEnd && PlayerInstance.GetComponent<BaseEntity>().actionEnd)
                    {
                        SetStage(GameStageType.ActionEnd);
                    }
                    else if(PlayerInstance.GetComponent<BaseEntity>().actionEnd)
                    {
                        SetStage(GameStageType.EnemyAction);
                    }
                    else
                    {
                        SetStage(GameStageType.PlayerAction);
                    }
                    this.ActionFlag = false;
                }
                break;
            case GameStageType.ActionEnd:
                // 按照顺序出发卡片效果
                ActionCardExcute();
                EnemyInstance.GetComponent<BaseEntity>().Restart();
                PlayerInstance.GetComponent<BaseEntity>().Restart();
                SetStage(GameStageType.GameStart);
                break;
        }

        // 如敌人死亡进入reward界面
        if (EnemyInstance.GetComponent<BaseEntity>().isDead() && GameStage != GameStageType.Tayin)
        {
            Debug.Log("Enemy is dead");
            SetStage(GameStageType.Tayin);
            this.TayinPanel.SetActive(true);
        }
        // 如果角色死亡进入gameover界面
        if (PlayerInstance.GetComponent<BaseEntity>().isDead() && GameStage != GameStageType.GameEnd)
        {
            Debug.Log("Player is dead");
            SetStage(GameStageType.GameEnd);
        }
    }
    public void SetStage(GameStageType gameStage)
    {
        if (gameStage == GameStageType.NextLevel)
        {
            this.GameStage = gameStage;
            return;
        }
        this.GameStage = GameStageType.Wait;
        StartCoroutine(TransitionToStage(gameStage));
        // text.text = "修仙阶段：" + gameStage.ToString();
        // this.GameStage = gameStage;
    }

    
    private IEnumerator TransitionToStage(GameStageType gameStage)
    {
        isWaitingForTransition = true;
        
        // 显示即将切换到的阶段
        if (text != null)
        {
            text.text = "修仙阶段：" + gameStage.ToString();
        }
        
        // 等待指定的时间
        yield return new WaitForSeconds(stageTransitionDelay);
        
        // 实际切换阶段
        this.GameStage = gameStage;
        isWaitingForTransition = false;
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

    // 添加这个新方法来重新加载当前场景
    private void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
    public void DrawDone()
    {
        this.DrawFlag = true;
    }
    public void ActionDone()
    {
        this.ActionFlag = true;
    }
}
