using UnityEngine;
using Unity;
using System.Collections.Generic;

class Effect_4 : EffectEntity
{
    public int number;
    public int timing;
    public CardData cardData;
    public GameState gameState;
    public List<int> data;

    public void execute()
    {
        // 额外触发x次
        if (gameState.GameStage == timing){
            this.cardData.replyNum = data[0];
        }
    }
}