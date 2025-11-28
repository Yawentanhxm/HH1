using UnityEngine;
using Unity;
using System.Collections.Generic;

class Effect_2 : EffectEntity
{
    public int number;
    public int timing;
    public CardData cardData;
    public GameState gameState;
    public List<int> data;

    public void execute()
    {
        // 抽牌阶段视作数字x
        if (gameState.GameStage == timing){
            this.cardData.drawCardNum = data[0];
        }
    }
}