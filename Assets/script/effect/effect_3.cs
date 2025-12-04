using UnityEngine;
using Unity;
using System.Collections.Generic;

class Effect_3 : EffectEntity
{
    public int number = 3;
    public int timing = 11;

    public override void Execute()
    {
        // 该卡在行动阶段数值+x
        if (gameState.GameStage == timing){
            this.cardData.executeCardNum += data[0];
        }
    }
}