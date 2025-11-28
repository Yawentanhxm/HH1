using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CanvasGroup))]
public class Card: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("拖拽设置")]
    [SerializeField] public bool isDraggable = false;
    [SerializeField] private float dragAlpha = 0.8f;
    [SerializeField] private float returnDuration = 0.3f;
    
    // 自动获取的组件
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Vector3 originalWorldPosition;
    private Transform originalParent;
    public CardData cardData;
    
    // 释放者
    public BaseEntity ownerEntity;
    // 指向者
    public BaseEntity enemyEntity;
    
    // 拖拽状态
    public bool IsDragging { get; private set; }
    
    void Awake()
    {
        // 自动获取必要组件
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        // 自动查找Canvas
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }
        
        // 确保Image可以接收射线
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.raycastTarget = true;
        }
    }
    
    // 开始拖拽
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        IsDragging = true;
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        originalWorldPosition = rectTransform.position;
        
        // 设置拖拽视觉效果
        if (canvasGroup != null)
        {
            canvasGroup.alpha = dragAlpha;
            canvasGroup.blocksRaycasts = false; // 重要：避免阻挡后续射线检测
        }
        
        // 移动到顶层
        if (canvas != null)
        {
            transform.SetParent(canvas.transform);
        }
    }
    
    // 拖拽中
    public void OnDrag(PointerEventData eventData)
    {
        if (!IsDragging) return;
        
        // 更新位置
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }
    
    // 结束拖拽
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDragging) return;
        
        IsDragging = false;
        
        // 恢复视觉效果
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
        
        // 检查是否放置到有效区域
        bool dropSuccessful = CheckValidDropZone(eventData);
        
        if (!dropSuccessful)
        {
            // 返回原位置
            ReturnToOriginalPosition();
        }
    }
    
    // 检查放置区域
    private bool CheckValidDropZone(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null)
        {
            // 这里可以检查特定的放置区域
            // 例如：if (eventData.pointerEnter.CompareTag("DropZone"))
            Debug.Log($"尝试放置到: {eventData.pointerEnter.name}");
            
            // 简单示例：如果放置到任何UI元素上就算成功
            if (eventData.pointerEnter.GetComponent<CardDropZone>() != null)
            {
                // 保持在新位置
                this.execute(eventData.pointerEnter.name);
                return true;
            }
        }
        
        return false;
    }
    
    // 返回原位置
    private void ReturnToOriginalPosition()
    {
        StartCoroutine(ReturnToPositionRoutine());
    }
    
    private System.Collections.IEnumerator ReturnToPositionRoutine()
    {
        float elapsed = 0f;
        
        transform.SetParent(originalParent);
        Vector2 startPos = rectTransform.anchoredPosition;
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnDuration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t);
            Debug.Log("originalPosition pos" +   originalPosition);
            Debug.Log("startPos pos" +   startPos);
            yield return null;
        }
        // rectTransform.anchoredPosition = originalPosition;
    }
    
    // 外部控制方法
    public void SetDraggable(bool draggable)
    {
        isDraggable = draggable;
    }

    // 执行卡片效果
    public void execute(string type)
    {
        // 判断是那边执行
        if(type == "attack"){
            enemyEntity.GetComponent<BaseEntity>().reduceHP(this.cardData.cardNum);
        }else if(type == "shield"){
            ownerEntity.GetComponent<BaseEntity>().addShied(this.cardData.cardNum);
        }else if(type == "shu"){
            // ownerEntity.GetComponent<BaseEntity>().drawCard(this.cardData.cardNum);
        }
        // 根据不同执行类型发挥不同效果

        // 执行后销毁
        Destroy(this.gameObject);
    }
    
    public void setCardData(CardData cardData)
    {
        this.cardData = cardData;
        Transform left = this.transform.Find("CardBase/Content/left");
        if (left != null) {
            Text textComponent = left.GetComponent<Text>();

            if (textComponent!=null){
                textComponent.text = cardData.cardName;
                textComponent.fontSize  = 14;
            }
        }

        
        Transform right = this.transform.Find("CardBase/Content/right");
        if (right != null) {
            Text textComponent = right.GetComponent<Text>();

            if (textComponent!=null){
                textComponent.text = cardData.cardName;
                textComponent.fontSize  = 14;
            }
        }
    }
}