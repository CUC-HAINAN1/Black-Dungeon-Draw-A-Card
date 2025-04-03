using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

<<<<<<< HEAD
public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{   
=======
public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
>>>>>>> caf4e57 (In card use and indicators developing)
    private CardStateManager cardStateManager;
    private CardQueueSystem cardQueueSystem;
    private RangeIndicatorManager rangeIndicatorManager;

    private Vector3 startPosition;
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    private Vector3 originalRotation;
<<<<<<< HEAD
    private bool isInPlayArea = false;
=======
>>>>>>> caf4e57 (In card use and indicators developing)
    private bool isAnimating = false;
    private bool isValidDrag = false;

<<<<<<< HEAD
    //处理范围指示器的部分
    private CardDataBase cardData;
    private Vector2 startDragPosition;

    [Header("动画设置")]
=======
    private CardDataBase cardData;

    [Header("Animation Settings")]
>>>>>>> caf4e57 (In card use and indicators developing)
    public float returnDuration = 0.3f;
    public Ease moveEase = Ease.OutBack;
    public float rotationDuration = 0.2f;
    [SerializeField] private float zoomScale = 0.3f;
    [SerializeField] private float hoverDuration = 0.3f;
    [SerializeField] private Ease hoverEase = Ease.OutBack;

    private void Awake() {
<<<<<<< HEAD
        
=======

>>>>>>> caf4e57 (In card use and indicators developing)
        cardStateManager = CardStateManager.Instance;
        cardQueueSystem = CardQueueSystem.Instance;
        rangeIndicatorManager = RangeIndicatorManager.Instance;

        originalScale = Vector3.one;
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

    }

    public void OnBeginDrag(PointerEventData eventData) {
<<<<<<< HEAD

        cardData = cardStateManager.GetCardState(gameObject).CardData;
        cardStateManager.SetDraggingState(gameObject, true);
        cardQueueSystem.SetCardQueueDraggingState(true);
=======
>>>>>>> caf4e57 (In card use and indicators developing)

        if (eventData.button != PointerEventData.InputButton.Left || 
            !cardStateManager.IsCardUsable(gameObject)) {
            
            eventData.pointerDrag = null; // 阻止事件传递
            return;
        
        }

        // 标记有效拖拽开始
        isValidDrag = true;
        Time.timeScale = 0.25f;

        cardData = cardStateManager.GetCardState(gameObject).CardData;
        cardStateManager.SetDraggingState(gameObject, true);
        cardQueueSystem.SetCardQueueDraggingState(true);

        if (isAnimating) return;

        startPosition = transform.position;
        originalRotation = transform.localEulerAngles;
        originalParent = transform.parent;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

<<<<<<< HEAD
=======
        rangeIndicatorManager.CreateIndicator(cardData);
        transform.DOScale(originalScale * zoomScale, hoverDuration).SetEase(hoverEase);

>>>>>>> caf4e57 (In card use and indicators developing)
    }

    public void OnDrag(PointerEventData eventData) {}

    private void Update() {

        if ((isValidDrag && cardStateManager.GetCardState(gameObject).IsDragging && Input.GetMouseButtonDown(1)) ||
            !cardStateManager.IsCardUsable(gameObject)
        ) {
            
            Time.timeScale = 1;
            CancelDrag();
            isValidDrag = false;
        
        }

        if (isValidDrag && cardStateManager.GetCardState(gameObject).IsDragging) {
            
            transform.position = Input.mousePosition;
            rangeIndicatorManager.UpdateIndicator();
        
<<<<<<< HEAD
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

=======
>>>>>>> caf4e57 (In card use and indicators developing)
        }

    }

    public void OnEndDrag(PointerEventData eventData) {
<<<<<<< HEAD
        
=======

        if (!isValidDrag || eventData.button != PointerEventData.InputButton.Left) {
           
            eventData.pointerDrag = null;
            return;

        }

        Time.timeScale = 1;

        CompleteCardUse();
        isValidDrag = false; // 重置标志

    }

    private void CompleteCardUse() {

>>>>>>> caf4e57 (In card use and indicators developing)
        cardStateManager.SetDraggingState(gameObject, false);
        cardQueueSystem.SetCardQueueDraggingState(false);

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        cardQueueSystem.TryUseCard(this);


        rangeIndicatorManager.ClearIndicator();
        RestoreOtherCards();

<<<<<<< HEAD
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
    
=======
>>>>>>> caf4e57 (In card use and indicators developing)
    }

    private void CancelDrag() {

        if (!cardStateManager.GetCardState(gameObject).IsDragging)
            return;

        cardStateManager.SetDraggingState(gameObject, false);
        cardQueueSystem.SetCardQueueDraggingState(false);

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        rangeIndicatorManager.ClearIndicator();
        ReturnToOriginalPosition();

    }

    private void ReturnToOriginalPosition() {
        
        transform.SetParent(originalParent);

        Sequence returnSequence = DOTween.Sequence();
        returnSequence
            .Append(transform.DOMove(startPosition, returnDuration).SetEase(moveEase))
            .Join(transform.DOLocalRotate(originalRotation, rotationDuration))
            .Join(transform.DOScale(originalScale, hoverDuration).SetEase(hoverEase))
            .OnStart(() => {
                canvasGroup.blocksRaycasts = false;
                isAnimating = true;
            })
            .OnComplete(() => {
                canvasGroup.blocksRaycasts = true;
                isAnimating = false;
            });
    }

    private void RestoreOtherCards() {
        foreach (var cardInfo in cardQueueSystem.currentCards) {
            if (cardInfo.cardInstance != null && cardInfo.cardInstance != gameObject) {
                cardInfo.cardInstance.transform.DOScale(originalScale, hoverDuration)
                    .SetEase(hoverEase);
            }
        }
    }

}