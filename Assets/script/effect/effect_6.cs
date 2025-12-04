using UnityEngine;
using Unity;
using System.Collections.Generic;

class Effect_6 : EffectEntity
{
    // 导表参数
    public int number = 1;
    public int timing = 4;

    // 游戏实例化时赋值
    [Header("拓印卡牌")]
    public CardData cardData;
    [Header("游戏当前状态参数")]
    public GameState gameState;
    [Header("效果参数")]
    public List<int> data;

    // 实例化时赋值
    [Header("持有对象")]
    public BaseEntity ownerEntity;
    [Header("敌对对象")]
    public BaseEntity enemyEntity;
    
    public void start()
    {
        ownerEntity = gameState.PlayerInstance.GetComponent<BaseEntity>();
        ownerEntity = gameState.EnemyInstance.GetComponent<BaseEntity>();
    }


    public override  void Execute()
    {
        // 攻击伤害增加x buff层数，每攻击x次，x buff层数减少z
        if (gameState.GameStage == timing){
            if (ownerEntity.BuffList.Count > 0)
            {
                int i = 0; 
                while (i < ownerEntity.BuffList.Count)
                {
                    if (ownerEntity.BuffList[i].GetComponent<BuffEntity>().number == data[0])
                    {
                        cardData.attackDamage += ownerEntity.BuffList[i].GetComponent<BuffEntity>().layer;
                        // 层数减1
                        ownerEntity.BuffList[i].GetComponent<BuffEntity>().layer-= data[1];
                        if (ownerEntity.BuffList[i].GetComponent<BuffEntity>().layer <= 0)
                        {
                            ownerEntity.BuffList.Remove(ownerEntity.BuffList[i]);
                            i--;
                        }
                    }
                     i++;
                }
            }
        }
    }
}