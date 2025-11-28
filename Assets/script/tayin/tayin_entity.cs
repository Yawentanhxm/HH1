using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TayinEntity : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("拓印技能ID")]
    public int id;
    [Header("拓印技能名称")]
    public string Name;
    [Header("效果实体")]
    public EffectEntity effectEntity;
    [Header("参数列表")]
    public List<int> data;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Init(int id, string name, EffectEntity effectEntity, List<int> data)
    {
        this.id = id;
        this.Name = name;
        this.effectEntity = effectEntity;
        this.data = data ?? new List<int>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 记录原始位置和父对象
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        
        // 设置canvas group属性以便拖拽时可以移动
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
        
        // 将对象置于最前
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 跟随鼠标移动
        rectTransform.anchoredPosition += eventData.delta / transform.root.GetComponent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 恢复canvas group属性
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        
        // 检查是否有目标卡片接收
        GameObject dropTarget = eventData.pointerEnter;
        if (dropTarget != null && dropTarget.CompareTag("Card"))
        {
            // 成功拖拽到卡片上
            CardDropTarget cardDropTarget = dropTarget.GetComponent<CardDropTarget>();
            if (cardDropTarget != null)
            {
                cardDropTarget.AssignTayin(this);
                // 不销毁对象，而是隐藏它
                gameObject.SetActive(false);
                return;
            }
        }
        
        // 如果没有拖拽到有效目标，则回到原位
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
    }
}