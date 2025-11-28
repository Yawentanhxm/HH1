using UnityEngine;
using Unity;
using System.Collections.Generic;

public class EffectEntity
{
    // 导表参数
    public int number = 1;
    public int timing = 4;

    // 游戏实例化时赋值
    [Header("拓印卡牌")]
    public Card card;
    [Header("游戏当前状态参数")]
    public GameState gameState;
    [Header("效果参数")]
    public List<int> data;

    public void init(int number, int timing, Card card, GameState gameState, List<int> data)
    {
        this.number = number;
        this.timing = timing;
        this.card = card;
        this.gameState = gameState;
        this.data = data;
    }

    public void excute()
    {

    }
}