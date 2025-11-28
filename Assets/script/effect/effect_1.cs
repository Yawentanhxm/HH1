using UnityEngine;
using Unity;
using System.Collections.Generic;

class Effect_1 : EffectEntity
{
    public int number;
    public int timing;
    public CardData cardData;
    public GameState gameState;
    public List<int> data;

    public void execute()
    {
        // 修改该卡属性为x
        if (gameState.GameStage == timing){
            this.cardData.property = data[0];
        }
    }
}