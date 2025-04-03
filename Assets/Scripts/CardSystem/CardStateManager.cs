using UnityEngine;
using System.Collections.Generic;
public class CardStateManager : MonoBehaviour
{
    public static CardStateManager Instance { get; private set; }

    // 存储所有卡牌状态 <实例ID, 状态>
    private Dictionary<GameObject, CardState> cardStates = new Dictionary<GameObject, CardState>();

    private void Awake() {
    
        if (Instance != null && Instance != this) {
        
            Destroy(gameObject);
        
        }
        else {
        
            Instance = this;
        
        }
    }

    // 注册新卡牌
    public void RegisterCard(GameObject cardInstance, bool isDragging, bool isUsing, CardDataBase cardData) {
    
        if (!cardStates.ContainsKey(cardInstance)) {
        
            cardStates.Add(cardInstance, new CardState(isDragging, isUsing, cardData));
        
        }
    
    }

    // 更新拖拽状态
    public void SetDraggingState(GameObject cardInstance, bool isDragging) {
    
        if (cardStates.ContainsKey(cardInstance)) {
        
            cardStates[cardInstance].IsDragging = isDragging;
        
        }
    
    }

    // 更新使用状态
    public void SetUsingState(GameObject cardInstance, bool isUsing) {
    
        if (cardStates.ContainsKey(cardInstance)) {
        
            cardStates[cardInstance].IsUsing = isUsing;
        }
    
    }

    // 获取完整状态
    public CardState GetCardState(GameObject cardInstance) {
        
        return cardStates.ContainsKey(cardInstance) ? cardStates[cardInstance] : null;
    
    }

    public bool IsCardUsable(GameObject cardInstance) {
        
        if (!cardStates.ContainsKey(cardInstance)) return false;

        return PlayerAttributes.Instance.Mana >= cardStates[cardInstance].CardData.manaCost;

    }

}

// 卡牌状态数据类
[System.Serializable]
public class CardState {

    public CardState(bool isDragging, bool isUsing, CardDataBase cardData) {

        IsDragging= isDragging;
        IsUsing = isUsing;
        CardData = cardData;

    }

    public bool IsDragging;
    public bool IsUsing;
    public CardDataBase CardData;

}