using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    private CardStateManager cardStateManager;
    private CardQueueSystem cardQueueSystem;
    private RangeIndicatorManager rangeIndicatorManager;

    private Vector3 startPosition;
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    private Vector3 originalRotation;
    private bool isAnimating = false;
    private bool isValidDrag = false;
    public bool IsDestroyed { get; private set; }

    private bool IsComponentAlive => !IsDestroyed && this != null;

    private CardDataBase cardData;

    [Header("动画设置")]
    public float returnDuration = 0.3f;
    public Ease moveEase = Ease.OutBack;
    public float rotationDuration = 0.2f;
    [SerializeField] private float zoomScale = 0.3f;
    [SerializeField] private float hoverDuration = 0.3f;
    [SerializeField] private Ease hoverEase = Ease.OutBack;

    private void Awake() {

        cardStateManager = CardStateManager.Instance;
        cardQueueSystem = CardQueueSystem.Instance;
        rangeIndicatorManager = RangeIndicatorManager.Instance;

        originalScale = Vector3.one;
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

    }

    public void OnPointerDown(PointerEventData eventData) {

        if (!IsComponentAlive || !cardStateManager.IsCardUsable(gameObject) || IsDestroyed ||
            eventData.button != PointerEventData.InputButton.Left)
            return;

        Time.timeScale = 0.25f;
        isValidDrag = true;

    }

    public void OnBeginDrag(PointerEventData eventData) {

        if (!IsComponentAlive || eventData.button != PointerEventData.InputButton.Left
        || !cardStateManager.IsCardUsable(gameObject) || IsDestroyed || cardQueueSystem.IsAnyCardDragging) {

            eventData.pointerDrag = null; // 拦截非左键或不可用的拖拽
            return;

        }

        // 标记有效拖拽开始
        isValidDrag = true;
        Time.timeScale = 0.25f;

        cardData = cardStateManager.GetCardState(gameObject).CardData;
        cardStateManager.SetDraggingState(gameObject, true);
        cardQueueSystem.SetCardQueueDraggingState(true);

        if (isAnimating)
            return;

        startPosition = transform.position;
        originalRotation = transform.localEulerAngles;
        originalParent = transform.parent;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        if (transform == null) {

            CustomLogger.LogError("目标 Transform 为空，无法启动动画！");
            return;

        }

        rangeIndicatorManager.CreateIndicator(cardData);
        transform.DOScale(originalScale * zoomScale, hoverDuration).SetEase(hoverEase);

    }

    public void OnDrag(PointerEventData eventData) {

        if (isValidDrag)
            Time.timeScale = 0.25f;

     }

    private void Update() {

        if ((isValidDrag && cardStateManager.GetCardState(gameObject).IsDragging && Input.GetMouseButtonDown(1)) ||
            !cardStateManager.IsCardUsable(gameObject)
        ) {

            Time.timeScale = 1;
            CancelDrag();
            isValidDrag = false;

        }

        if (isValidDrag && cardStateManager.GetCardState(gameObject).IsDragging) {

            Time.timeScale = 0.25f;
            transform.position = Input.mousePosition;
            rangeIndicatorManager.UpdateIndicator();

        }

    }

    public void OnEndDrag(PointerEventData eventData) {

        if (!isValidDrag || eventData.button != PointerEventData.InputButton.Left) {

            eventData.pointerDrag = null;
            return;

        }

        Time.timeScale = 1;

        CompleteCardUse();
        isValidDrag = false; // 重置标志

    }

    private void CompleteCardUse() {

        cardStateManager.SetDraggingState(gameObject, false);
        cardQueueSystem.SetCardQueueDraggingState(false);

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (!cardQueueSystem.TryUseCard(gameObject))
            ReturnToOriginalPosition();

        //技能使用！
        SkillSystem.Instance.ExecuteSkill(cardStateManager.GetCardState(gameObject).CardData);

        rangeIndicatorManager.ClearIndicator();
        RestoreOtherCards();

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

        if (transform == null) {

            CustomLogger.LogError("目标 Transform 为空，无法启动动画！");
            return;

        }

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

            if (cardInfo.cardInstance != null &&
                cardInfo.cardInstance.transform != null &&
                cardInfo.cardInstance != gameObject) {

                cardInfo.cardInstance.transform.DOScale(originalScale, hoverDuration)
                    .SetEase(hoverEase);

            }
        }
    }

    public void DestroyThis() {

        IsDestroyed = true;

        if (TryGetComponent<CanvasGroup>(out var cg))
            cg.blocksRaycasts = true;

    }

    private void OnDestroy() {

        // 确保所有引用置空
        cardStateManager = null;
        cardQueueSystem = null;
        rangeIndicatorManager = null;

    }

}
