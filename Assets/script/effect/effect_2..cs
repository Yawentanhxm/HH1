using UnityEngine;
using Unity;
using System.Collections.Generic;

class Effect_2 : EffectEntity
{
    public int number = 2;
    public int timing = 11;

    public override void Execute()
    {
        // 抽牌阶段视作数字x
        if (gameState.GameStage == timing){
            this.cardData.drawCardNum = data[0];
        }
    }
}