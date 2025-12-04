using UnityEngine;
using Unity;
using System.Collections.Generic;

class Effect_5 : EffectEntity
{
    // 导表参数
    public int number = 1;
    public int timing = 4;

    // 游戏实例化时赋值
    [Header("拓印卡牌")]
    public CardData cardData;
    [Header("游戏当前状态参数")]
    public GameState gameState;
    [Header("效果参数")]
    public List<int> data;

    public override  void Execute()
    {
        // 给予敌方x层y buff
        if (gameState.GameStage == timing){
            gameState.EnemyInstance.GetComponent<BaseEntity>().addBuff(data[0], data[1]);
        }
    }
}