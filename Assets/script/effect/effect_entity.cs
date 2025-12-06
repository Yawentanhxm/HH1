using UnityEngine;
using Unity;
using System.Collections.Generic;

public abstract class EffectEntity
{
    // 导表参数
    public int number;
    public GameStageType timing;

    // 游戏实例化时赋值
    [Header("拓印卡牌")]
    public CardData cardData;
    [Header("游戏当前状态参数")]
    public GameState gameState;
    [Header("效果参数")]
    public List<int> data;

    public void init(CardData cardData, GameState gameState, List<int> data)
    {
        this.number = number;
        this.timing = timing;
        this.cardData = cardData;
        this.gameState = gameState;
        this.data = data;
    }

    public abstract void Execute();
}