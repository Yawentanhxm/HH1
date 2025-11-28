using UnityEngine;
using UnityEngine.UI;
using System;

public class CardDropTarget : MonoBehaviour
{
    [Header("关联的卡片数据")]
    public CardData cardData;
    
    [Header("显示拓印信息的UI元素")]
    public Text tayinNameText;
    public Image tayinIcon;
    public GameObject tayinInfoPanel;

    public GameState gameState;
    
    private TayinEntity assignedTayin;
    
    private void Start()
    {
        if (tayinInfoPanel != null)
        {
            tayinInfoPanel.SetActive(false);
        }
        cardData = this.GetComponent<Card>().cardData;
        gameState = GameObject.Find("GameManage").GetComponent<GameState>();
    }
    
    /// <summary>
    /// 分配拓印给这张卡片
    /// </summary>
    /// <param name="tayin">拓印实体</param>
    public void AssignTayin(TayinEntity tayin)
    {
        if (tayin == null) return;
        
        assignedTayin = tayin;
        
        // 更新卡片数据中的拓印ID
        if (cardData != null)
        {
            cardData.tayinId = tayin.id;
            SaveToPlayerLibrary();
        }
        
        // 更新UI显示
        UpdateTayinDisplay(tayin);
        
        Debug.Log($"拓印 {tayin.Name} 已分配给卡片 {cardData?.cardName ?? "未知"}");
    }
    
    /// <summary>
    /// 更新拓印显示信息
    /// </summary>
    /// <param name="tayin">拓印实体</param>
    private void UpdateTayinDisplay(TayinEntity tayin)
    {
        if (tayinInfoPanel != null)
        {
            tayinInfoPanel.SetActive(true);
        }
        
        if (tayinNameText != null)
        {
            tayinNameText.text = tayin.Name;
        }
        
        // 如果有图标，可以在这里设置
        // if (tayinIcon != null)
        // {
        //     tayinIcon.sprite = someSprite;
        // }
    }
    
    /// <summary>
    /// 保存到玩家牌库JSON文件
    /// </summary>
    private void SaveToPlayerLibrary()
    {
        // 查找CardLibrary实例并保存
        CardLibary cardLibrary = gameState.cardLibary;
        if (cardLibrary != null)
        {
            cardLibrary.SaveCardData(cardData);
            Debug.LogWarning("保存数据成功");
        }
        else
        {
            Debug.LogWarning("找不到CardLibrary实例，无法保存数据");
        }
    }
}