using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CanvasGroup))]
public class Card: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler// IPointerEnterHandler, IPointerExitHandler
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
    
    CardLibary cardLibary = new CardLibary();

    [Header("悬停显示设置")]
    public GameObject hoverInfoPanel; // 悬停信息面板
    // public TextMeshPro descriptionText; // 描述文本组件
    public Vector3 hoverOffset; // 悬停面板偏移
    
    [Header("动画设置")]
    public float fadeInDuration = 0.3f;
    public float scaleAmount = 1.1f;
    
    private Vector3 originalScale;
    private bool isHovering = false;
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
    void Start()
    {
        
        if (hoverInfoPanel != null)
        {
            hoverInfoPanel.SetActive(true);
        }
        else
        {
            // 更安全的获取子对象方式
            Transform hoverInfoTransform = this.transform.Find("CardBase/HoverInfo");
            if (hoverInfoTransform != null)
            {
                hoverInfoPanel = hoverInfoTransform.gameObject;
                hoverInfoPanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("未找到名为 'HoverInfo' 的子对象", this);
            }
        }
        hoverOffset = new Vector3(100, 0, 0);
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

    public void InitCardInstance()
    {
        Transform left = this.transform.Find("CardBase/Content/left");
        if (left != null) {
            Text textComponent = left.GetComponent<Text>();

            if (textComponent!=null){
                textComponent.text = this.cardData.cardName;
                textComponent.fontSize  = 40;
            }
        }

        
        Transform right = this.transform.Find("CardBase/Content/right");
        if (right != null) {
            Text textComponent = right.GetComponent<Text>();

            if (textComponent!=null){
                textComponent.text = this.cardData.cardName;
                textComponent.fontSize  = 40;
            }
        }

        Transform tayinDesc = this.transform.Find("CardBase/HoverInfo");
        if (tayinDesc != null) {
            Text textComponent = tayinDesc.GetComponent<Text>();

            if (textComponent!=null){
                textComponent.text = this.cardData.description;
                textComponent.fontSize  = 14;
            }
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

    // 放置卡片
    public void execute(string type)
    {
        // 判断是那边执行
        if(type == "attack"){
            ownerEntity.GetComponent<BaseEntity>().attackCard.Add(this.gameObject);
            // enemyEntity.GetComponent<BaseEntity>().reduceHP(this.cardData.cardNum);
        }else if(type == "shield"){
            ownerEntity.GetComponent<BaseEntity>().defCard.Add(this.gameObject);
            // ownerEntity.GetComponent<BaseEntity>().addShied(this.cardData.cardNum);
        }else if(type == "shu"){
            // ownerEntity.GetComponent<BaseEntity>().drawCard(this.cardData.cardNum);
        }
        // 拖拽完敌人行动
        if (ownerEntity.GetComponent<BaseEntity>().handCardPrefab.Contains(this.gameObject))
        {
            ownerEntity.GetComponent<BaseEntity>().actionCardPrefab.Add(this.gameObject);
            ownerEntity.GetComponent<BaseEntity>().handCardPrefab.Remove(this.gameObject);
        }
        ownerEntity.GetComponent<BaseEntity>().gameState.GameStage = 2;
        // 根据不同执行类型发挥不同效果

    }
    
    public void actionCard()
    {
        if(ownerEntity.GetComponent<BaseEntity>().attackCard.Contains(this.gameObject))
        {
            enemyEntity.GetComponent<BaseEntity>().reduceHP(this.cardData.cardNum);
        }else if(ownerEntity.GetComponent<BaseEntity>().defCard.Contains(this.gameObject))
        {
            ownerEntity.GetComponent<BaseEntity>().addShied(this.cardData.cardNum);
        }
        // // 判断是那边执行
        // if(type == "attack"){
        // }else if(type == "shield"){
        // }else if(type == "shu"){
        //     // ownerEntity.GetComponent<BaseEntity>().drawCard(this.cardData.cardNum);
        // }
        
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

    // 将data数据保存到library中
    public void SaveToLibrary()
    {
        cardLibary.SaveCardData(this.cardData);
    }

    // 鼠标进入时调用
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        ShowHoverInfo();
        // StartHoverAnimation();
    }

    // 鼠标离开时调用
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        HideHoverInfo();
        // StopHoverAnimation();
    }

    void ShowHoverInfo()
    {
        if (hoverInfoPanel != null)
        {
            // 设置悬停面板位置
            hoverInfoPanel.transform.position = this.transform.position + hoverOffset;
            hoverInfoPanel.SetActive(true);
            
            // 启动淡入动画
            // StartCoroutine(FadeInPanel());
        }
    }

    void HideHoverInfo()
    {
        if (hoverInfoPanel != null)
        {
            hoverInfoPanel.SetActive(false);
        }
    }

    void StartHoverAnimation()
    {
        // 卡片放大效果
        LeanTween.scale(gameObject, originalScale * scaleAmount, 0.2f)
                 .setEase(LeanTweenType.easeOutBack);
    }

    void StopHoverAnimation()
    {
        // 恢复原始大小
        LeanTween.scale(gameObject, originalScale, 0.2f)
                 .setEase(LeanTweenType.easeInOutCubic);
    }

    System.Collections.IEnumerator FadeInPanel()
    {
        if (hoverInfoPanel != null)
        {
            CanvasGroup canvasGroup = hoverInfoPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = hoverInfoPanel.AddComponent<CanvasGroup>();
            }
            
            canvasGroup.alpha = 0f;
            float timer = 0f;
            
            while (timer < fadeInDuration && isHovering)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
                timer += Time.deltaTime;
                yield return null;
            }
            
            if (isHovering)
            {
                canvasGroup.alpha = 1f;
            }
        }
    }
}