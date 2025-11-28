// CardData.cs
using System;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu(fileName = "CardData", menuName = "Card Game/Card Data")]
[System.Serializable]
public class CardData
{
    [Header("基础信息")]
    public string id;
    public string cardName;
    public int cardNum;
    public int drawCardNum;
    public int executeCardNum;
    public int attackDamage;
    public int replyNum = 0;
    public int tayinId;
    // 1:地。2：火：3：水，4：风，5： 任意
    public int property;
    [TextArea(3, 5)]
    public string description;
    public CardType cardType;
    public CardRarity rarity;
    // public int manaCost;
    
    [Header("视觉效果")]
    // public Sprite cardArt;
    // public Sprite frameSprite;
    // public Color frameColor = Color.white;
    
    [Header("单位卡片属性")]
    public int attack;
    // public int health;
    // public int defense;
    
    // [Header("法术卡片属性")]
    // public List<CardEffect> effects;
    
    // [Header("预制体引用")]
    public GameObject cardPrefab;
}

public enum CardType
{
    Small,
    Big
}

public enum CardRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public class CardEffect
{
    public EffectType effectType;
    public int value;
    public string target;
}

public enum EffectType
{
    Damage,
    Heal,
    DrawCard,
    BuffAttack,
    BuffHealth
}