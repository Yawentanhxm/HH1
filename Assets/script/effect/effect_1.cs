using UnityEngine;
using Unity;
using System.Collections.Generic;

class Effect_1 : EffectEntity
{
    public int number = 1;
    public int timing = 11;

    public override void Execute()
    {
        // 修改该卡属性为x
        if (gameState.GameStage == timing){
            this.cardData.property = data[0];
        }
    }
}