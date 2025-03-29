using UnityEngine;

public class CardQueueSystem : MonoBehaviour
{
    [Header("卡牌位置配置")]
    public Transform[] cardSlots = new Transform[4];
    
    [System.Serializable]
    public class ActiveCardInfo {
    
        public CardDataBase cardData;
        public GameObject cardInstance;
    
    }

    [Header("当前卡牌状态")]
    [SerializeField] public ActiveCardInfo[] currentCards = new ActiveCardInfo[4];
    
    public int currentSlotIndex = 0;
    public void Initialize() {
    
        ClearAllSlots();
    
    }

    public CardDataBase DrawCard() {
    
        return CardPoolManager.Instance.DrawCard();
    
    }

    public void CreateCard(CardDataBase data, Transform targetSlot) {
    
        int slotIndex = System.Array.IndexOf(cardSlots, targetSlot);
        
        if (slotIndex == -1) {
        
            Debug.LogError("Target slot not found!");
            return;
        }
        
        ClearSlot(targetSlot);

        GameObject newCard = Instantiate(
            
            cardPrefab,
            targetSlot.position,
            targetSlot.rotation,
            targetSlot
        
        );

        newCard.transform.SetParent(targetSlot, false);

        currentCards[slotIndex] = new ActiveCardInfo {
        
            cardData = data,
            cardInstance = newCard
        
        };
    }

    public Transform GetNextSlot() {
    
        Transform slot = cardSlots[currentSlotIndex];
        currentSlotIndex = (currentSlotIndex + 1) % cardSlots.Length;
        return slot;
    
    }

    private void ClearAllSlots() {
    
        foreach(Transform slot in cardSlots) {
        
            ClearSlot(slot);
        
        }
    }

    private void ClearSlot(Transform slot) {
    
        foreach(Transform child in slot) {
        
            Destroy(child.gameObject);
        
        }
    
    }

    [Header("基础配置")]
    public GameObject cardPrefab;

}