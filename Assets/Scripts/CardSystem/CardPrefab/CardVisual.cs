using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;


public class CardVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [Header("组件引用")]
    [SerializeField] private Image cardImage; 
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("基于鼠标放置的放大与缩放设置")]
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float ZoomScale = 0.9f;
    [SerializeField] private float hoverDuration = 0.3f;
    [SerializeField] private Ease hoverEase = Ease.OutBack;

    private Vector3 originalScale;
    private void Awake() {
       
        originalScale = Vector3.one;

    }

    public void OnPointerEnter(PointerEventData eventData) {
    
        if (CardQueueSystem.Instance == null || 
            CardStateManager.Instance.GetCardState(gameObject).IsDragging ||
            CardStateManager.Instance.GetCardState(gameObject).IsUsing ||
            CardQueueSystem.Instance.IsAnyCardDragging
        ) return;
        
        // 放大当前卡牌
        transform.DOScale(originalScale * hoverScale, hoverDuration)
            .SetEase(hoverEase);

        // 缩小其他卡牌
        foreach (var cardInfo in CardQueueSystem.Instance.currentCards) {
            
            if (cardInfo.cardInstance != null && cardInfo.cardInstance != gameObject) {
                
                cardInfo.cardInstance.transform.DOScale(originalScale * ZoomScale, hoverDuration)
                    .SetEase(hoverEase);
            
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CardQueueSystem.Instance == null || 
            CardStateManager.Instance.GetCardState(gameObject).IsDragging ||
            CardStateManager.Instance.GetCardState(gameObject).IsUsing ||
            CardQueueSystem.Instance.IsAnyCardDragging
        ) return;

        // 恢复当前卡牌大小
        transform.DOScale(originalScale, hoverDuration)
            .SetEase(hoverEase);

        // 恢复其他卡牌大小
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
        
        // 根据稀有度改变边框颜色
        GetComponent<Image>().color = data.rarity == CardDataBase.Rarity.Rare 
            ? Color.yellow 
            : Color.white;
    
    }

}