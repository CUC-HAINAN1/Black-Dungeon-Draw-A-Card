using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class CardVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [Header("组件引用")]
    [SerializeField] private Image cardImage; 
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI ManaCostText;

    [Header("悬停缩放设置")]
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float otherCardScale = 0.9f;
    [SerializeField] private float hoverDuration = 0.3f;
    [SerializeField] private Ease hoverEase = Ease.OutBack;

    private Vector3 originalScale;
    private bool isHovering;
    private bool wasUsableLastFrame;

    private void Awake() {
        
        originalScale = Vector3.one;
    
    }

    private void Update() {
       
        bool isUsable = CardStateManager.Instance.IsCardUsable(gameObject);

        if (isUsable != wasUsableLastFrame) {
            
            UpdateHoverState();
            wasUsableLastFrame = isUsable;
        
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        
        isHovering = true;
        UpdateHoverState();
    
    }

    public void OnPointerExit(PointerEventData eventData) {
        
        isHovering = false;
        UpdateHoverState();
    
    }

    private void UpdateHoverState() {
        
        if (CardQueueSystem.Instance.IsAnyCardDragging ||
            (CardQueueSystem.Instance.IsAnyCardHovering && 
            !CardStateManager.Instance.GetCardState(gameObject).IsHovering)
        ) return;
        
        if (ShouldIgnoreHover()) {
            
            ResetHoverEffect();
            return;
        
        }

        if (isHovering) {
            
            ApplyHoverEffect();
        
        } else {
            
            ResetHoverEffect();
        
        }
    }

    private bool ShouldIgnoreHover() {
        
        return CardQueueSystem.Instance == null || 
               CardStateManager.Instance.GetCardState(gameObject).IsDragging ||
               CardStateManager.Instance.GetCardState(gameObject).IsUsing ||
               CardQueueSystem.Instance.IsAnyCardDragging ||
               !CardStateManager.Instance.IsCardUsable(gameObject);
    
    }

    private void ApplyHoverEffect() {

        CardQueueSystem.Instance.SetCardQueueHoveringState(true);
        CardStateManager.Instance.SetHoveringState(gameObject, true);

        // 放大当前卡牌
        transform.DOScale(originalScale * hoverScale, hoverDuration)
            .SetEase(hoverEase);

        // 缩小其他卡牌
        foreach (var cardInfo in CardQueueSystem.Instance.currentCards) {
            if (cardInfo.cardInstance != null && cardInfo.cardInstance != gameObject) {
                
                cardInfo.cardInstance.transform.DOScale(originalScale * otherCardScale, hoverDuration)
                    .SetEase(hoverEase);
            
            }
        }
    }

    private void ResetHoverEffect() {

        CardQueueSystem.Instance.SetCardQueueHoveringState(false);
        CardStateManager.Instance.SetHoveringState(gameObject, false);

        // 恢复所有卡牌尺寸
        transform.DOScale(originalScale, hoverDuration).SetEase(hoverEase);
        
        foreach (var cardInfo in CardQueueSystem.Instance.currentCards) {
            if (cardInfo.cardInstance != null && cardInfo.cardInstance != gameObject) {
                
                cardInfo.cardInstance.transform.DOScale(originalScale, hoverDuration)
                    .SetEase(hoverEase);
            
            }
        }
    }

    public void Initialize(CardDataBase data) {
        
        cardImage.sprite = data.cardIcon;
        nameText.text = data.displayName;
        descriptionText.text = data.description;
        ManaCostText.text = data.manaCost.ToString();
    
    }

    public void CheckHoverAfterAnimation() {
    
    bool isMouseOver = RectTransformUtility.RectangleContainsScreenPoint(
        
        (RectTransform)transform,
        Input.mousePosition,
        null 
    
    );
    
        isHovering = isMouseOver;
        UpdateHoverState();
    
    }

}