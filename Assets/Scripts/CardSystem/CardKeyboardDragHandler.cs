using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class CardKeyboardDragHandler : MonoBehaviour {

    public static CardKeyboardDragHandler Instance { get; private set; }

    private GameObject CurrentDraggingCard;

    private Vector3 startPosition;
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    private Vector3 originalRotation;
    private bool isAnimating = false;
    public bool IsDestroyed { get; private set; }
    private Coroutine followMouseRoutine;

    private CardDataBase cardData;

    [Header("动画设置")]
    public float returnDuration = 0.3f;
    public Ease moveEase = Ease.OutBack;
    public float rotationDuration = 0.2f;
    [SerializeField] private float zoomScale = 0.3f;
    [SerializeField] private float hoverDuration = 0.3f;
    [SerializeField] private Ease hoverEase = Ease.OutBack;

    private void Awake() {

        if (Instance != null) {

            Destroy(gameObject);
            return;

        }

        Instance = this;
        ResetState();

    }


    private void OnKeyBoardBeginDrag() {

        if (CurrentDraggingCard != null)
            CancelDrag();

        originalScale = Vector3.one;
        canvasGroup = CurrentDraggingCard.GetComponent<CanvasGroup>() ?? CurrentDraggingCard.AddComponent<CanvasGroup>();

        Time.timeScale = 0.25f;

        cardData = CardStateManager.Instance.GetCardState(CurrentDraggingCard).CardData;
        CardStateManager.Instance.SetDraggingState(CurrentDraggingCard, true);
        CardQueueSystem.Instance.SetCardQueueDraggingState(true);

        if (isAnimating)
            return;

        startPosition = CurrentDraggingCard.transform.position;
        originalRotation = CurrentDraggingCard.transform.localEulerAngles;
        originalParent = CurrentDraggingCard.transform.parent;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        RangeIndicatorManager.Instance.CreateIndicator(cardData);
        CurrentDraggingCard.transform.DOScale(originalScale * zoomScale, hoverDuration).SetEase(hoverEase);

    }

    private void Update() {

        for (int i = 0; i < 4; i++) {

            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                if (!CardQueueSystem.Instance.IsAnyCardDragging &&
                    CardQueueSystem.Instance.currentCards[i] != null &&
                    CardQueueSystem.Instance.currentCards[i].cardInstance != null &&
                    CardStateManager.Instance.IsCardUsable(CardQueueSystem.Instance.currentCards[i].cardInstance)) {

                    CurrentDraggingCard = CardQueueSystem.Instance.currentCards[i].cardInstance;
                    OnKeyBoardBeginDrag();
                    StartFollowMouse();

                }
            }
        }

        if (CurrentDraggingCard != null && ((CardStateManager.Instance.GetCardState(CurrentDraggingCard).IsDragging && Input.GetMouseButtonDown(1)) ||
            !CardStateManager.Instance.IsCardUsable(CurrentDraggingCard))
        ) {

            Time.timeScale = 1;
            CancelDrag();

        }

        if (CurrentDraggingCard != null && CardStateManager.Instance.GetCardState(CurrentDraggingCard).IsDragging) {

            Time.timeScale = 0.25f;

            RangeIndicatorManager.Instance.UpdateIndicator();

        }

        if (CurrentDraggingCard != null && Input.GetMouseButtonDown(0)) {

            Time.timeScale = 1;
            CompleteCardUse();

        }

        if (!CardQueueSystem.Instance.IsAnyCardDragging && CurrentDraggingCard == null && Time.timeScale != 1f)
            Time.timeScale = 1f;

    }

    private void CompleteCardUse() {

        CardStateManager.Instance.SetDraggingState(CurrentDraggingCard, false);
        CardQueueSystem.Instance.SetCardQueueDraggingState(false);

        if (followMouseRoutine != null)
            StopCoroutine(followMouseRoutine);

        followMouseRoutine = null;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (!CardQueueSystem.Instance.TryUseCard(CurrentDraggingCard))
            ReturnToOriginalPosition();

        //技能使用！
        SkillSystem.Instance.ExecuteSkill(CardStateManager.Instance.GetCardState(CurrentDraggingCard).CardData);

        RangeIndicatorManager.Instance.ClearIndicator();
        ResetState();

    }

    private void CancelDrag() {

        if (!CardStateManager.Instance.GetCardState(CurrentDraggingCard).IsDragging)
            return;

        if (followMouseRoutine != null)
            StopCoroutine(followMouseRoutine);

        followMouseRoutine = null;

        CardStateManager.Instance.SetDraggingState(CurrentDraggingCard, false);
        CardQueueSystem.Instance.SetCardQueueDraggingState(false);

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        RangeIndicatorManager.Instance.ClearIndicator();
        ReturnToOriginalPosition();

    }

    private void ReturnToOriginalPosition() {

        CurrentDraggingCard.transform.SetParent(originalParent);

        Sequence returnSequence = DOTween.Sequence();
        returnSequence
            .Append(CurrentDraggingCard.transform.DOMove(startPosition, returnDuration).SetEase(moveEase))
            .Join(CurrentDraggingCard.transform.DOLocalRotate(originalRotation, rotationDuration))
            .OnStart(() => {
                canvasGroup.blocksRaycasts = false;
                isAnimating = true;
            })
            .OnComplete(() => {
                canvasGroup.blocksRaycasts = true;
                ResetState();
            });

    }
    private void StartFollowMouse() {

        if (followMouseRoutine != null)
            StopCoroutine(followMouseRoutine);

        followMouseRoutine = StartCoroutine(SmoothFollowMouse());

    }

    private IEnumerator SmoothFollowMouse() {

        float followSpeed = 10f;

        while (CurrentDraggingCard != null &&
            CardStateManager.Instance.GetCardState(CurrentDraggingCard).IsDragging) {

            Vector3 target = Input.mousePosition;
            Vector3 current = CurrentDraggingCard.transform.position;

            // 插值到目标位置
            CurrentDraggingCard.transform.position = Vector3.Lerp(current, target, Time.unscaledDeltaTime * followSpeed);

            yield return null;
        }

        followMouseRoutine = null;
    }

    private void ResetState() {

        CurrentDraggingCard = null;
        canvasGroup = null;
        isAnimating = false;
        Time.timeScale = 1f;
        

    }

}
