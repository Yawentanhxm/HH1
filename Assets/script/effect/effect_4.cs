using UnityEngine;
using Unity;
using System.Collections.Generic;

class Effect_4 : EffectEntity
{
    public int number = 4;
    public GameStageType timing = GameStageType.Tayin;

    public override  void Execute()
    {
        // 额外触发x次
        if (gameState.GameStage == timing){
            this.cardData.replyNum = data[0];
        }
    }
}