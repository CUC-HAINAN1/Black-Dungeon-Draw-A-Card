using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{   
    private CardStateManager cardStateManager;
    private CardQueueSystem cardQueueSystem;
    private RangeIndicatorManager rangeIndicatorManager;

    private Vector3 startPosition;
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private RectTransform playArea;
    private UnityEngine.UI.Image invalidplayAreaImage;
    private Vector3 originalScale;
    private Vector3 originalRotation;
    private bool isInPlayArea = false;
    private bool isAnimating = false;

    //处理范围指示器的部分
    private CardDataBase cardData;
    private Vector2 startDragPosition;

    [Header("动画设置")]
    public float returnDuration = 0.3f;
    public Ease moveEase = Ease.OutBack;
    public float rotationDuration = 0.2f;

    [Header("基于鼠标放置的放大与缩放设置")]
    [SerializeField] private float zoomScale = 0.3f;
    [SerializeField] private float hoverDuration = 0.3f;
    [SerializeField] private Ease hoverEase = Ease.OutBack;

    private void Awake() {
        
        cardStateManager = CardStateManager.Instance;
        cardQueueSystem = CardQueueSystem.Instance;
        rangeIndicatorManager = RangeIndicatorManager.Instance;

        originalScale = Vector3.one;

        GameObject playAreaObj = GameObject.Find("PlayArea");
        playArea = playAreaObj.GetComponent<RectTransform>();

        GameObject invalidplayAreaObj = GameObject.Find("InvalidPlayArea");
        invalidplayAreaImage = invalidplayAreaObj.GetComponent<UnityEngine.UI.Image>();
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    
    }

    public void OnBeginDrag(PointerEventData eventData) {

        cardData = cardStateManager.GetCardState(gameObject).CardData;
        cardStateManager.SetDraggingState(gameObject, true);
        cardQueueSystem.SetCardQueueDraggingState(true);

        if (isAnimating) return;    

        //将不合法区域的颜色变暗
        SetInvalidAreaColor();
        
        startPosition = transform.position;
        originalRotation = transform.localEulerAngles;
        originalParent = transform.parent;
        
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

    }

    public void OnDrag(PointerEventData eventData) {
        
        if (IsInPlayArea()) {

            if (!isInPlayArea) {

                transform.DOScale(originalScale * zoomScale, hoverDuration)
            .SetEase(hoverEase);

                rangeIndicatorManager.CreateIndicator(cardData);
                startDragPosition = eventData.position;

            }

            isInPlayArea = true;
            Vector2 delta = eventData.position - startDragPosition;
            rangeIndicatorManager.UpdateIndicator(delta);

        } else {

            if (isInPlayArea) {

                transform.DOScale(originalScale, hoverDuration)
            .SetEase(hoverEase);

            }

            isInPlayArea = false;
            rangeIndicatorManager.ClearIndicator();

        }

        transform.position = Input.mousePosition;
    
    }

    public void OnEndDrag(PointerEventData eventData) {
        
        cardStateManager.SetDraggingState(gameObject, false);
        cardQueueSystem.SetCardQueueDraggingState(false);

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        //重置不合法区域颜色
        ResetInvalidAreaColor();

        // 判断是否在有效区域
        if (IsInPlayArea()) {
        
            cardQueueSystem.TryUseCard(this);

            // 将最终参数传递给技能系统
            Vector3 direction = (eventData.position - startDragPosition).normalized;
            //SkillSystem.Instance.ExecuteSkill(cardData, direction);

        }
        else {
        
            ReturnToOriginalPosition();
        
        }

        
        //恢复其他卡牌大小
        foreach (var cardInfo in cardQueueSystem.currentCards) {
        
            if (cardInfo.cardInstance != null && cardInfo.cardInstance != gameObject) {
            
                cardInfo.cardInstance.transform.DOScale(originalScale, hoverDuration)
                    .SetEase(hoverEase);
            
            }
        
        }
    
    }

    private bool IsInPlayArea() {
    
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                
                playArea, 
                Input.mousePosition, 
                null, 
                out localPos
        
        );

        return playArea.rect.Contains(localPos);
    
    }

    private void ReturnToOriginalPosition() {

        // 立即重置父级关系
        transform.SetParent(originalParent);
        
        // 同时恢复位置和旋转
        Sequence returnSequence = DOTween.Sequence();
        returnSequence
            .Append(transform.DOMove(startPosition, returnDuration).SetEase(moveEase))
            .Join(transform.DOLocalRotate(originalRotation, rotationDuration))
            .OnStart(() => {
                
                canvasGroup.blocksRaycasts = false;
                isAnimating = true;
            
            })
            .OnComplete(() => {
                
                canvasGroup.blocksRaycasts = true;
                isAnimating = false;
                transform.localScale = Vector3.one;  
            
            });
    
    }

    private void SetInvalidAreaColor() {

        if (invalidplayAreaImage == null) Debug.Log("No Image");

        Color color = invalidplayAreaImage.color;
        color.a = 0.3f;
        invalidplayAreaImage.color = color;

    }

    private void ResetInvalidAreaColor() {

        Color color = invalidplayAreaImage.color;
        color.a = 0;
        invalidplayAreaImage.color = color;

    }

}