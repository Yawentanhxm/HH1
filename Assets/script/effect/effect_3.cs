using UnityEngine;
using Unity;
using System.Collections.Generic;

class Effect_3 : EffectEntity
{
    public int number;
    public int timing;
    public CardData cardData;
    public GameState gameState;
    public List<int> data;

    public void execute()
    {
        // 该卡在行动阶段数值+x
        if (gameState.GameStage == timing){
            this.cardData.executeCardNum += data[0];
        }
    }
}