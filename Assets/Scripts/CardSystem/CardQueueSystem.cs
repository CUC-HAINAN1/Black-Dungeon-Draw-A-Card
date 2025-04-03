using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class CardQueueSystem : MonoBehaviour {

    public static CardQueueSystem Instance {get; private set;}

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
    public bool IsAnyCardDragging {get; private set;}

    private void Awake() {
        
        if (Instance != this && Instance != null) {

            Destroy(gameObject);

        }

        else {

            Instance = this;
            Initialize();

        }

    }

    public void Initialize() {
    
        ClearAllSlots();
    
    }

    public CardDataBase DrawCard() {
    
        return CardPoolManager.Instance.DrawCard();
    
    }

    public void CreateCard(CardDataBase data, Transform targetSlot) {
    
        int slotIndex = System.Array.IndexOf(cardSlots, targetSlot);

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

        // 注册新卡牌状态
        CardStateManager.Instance.RegisterCard(newCard, false, false, data);
        
    }

    public Transform GetNextSlot() {
        
        int nextIndex = -1;
        for (int i = 0; i < currentCards.Length; i++) {

            if (currentCards[i] == null || currentCards[i].cardInstance == null) {

                nextIndex = i;
                break;

            }

        }

        return nextIndex == -1 ? null : cardSlots[nextIndex];

    }

    public bool TryUseCard(CardDragHandler draggedCard) {

        // 获取卡牌数据
        int slotIndex = System.Array.FindIndex(currentCards, 
            c => c.cardInstance == draggedCard.gameObject);

        if (slotIndex == -1) return false;

        // 执行卡牌使用逻辑
        StartCoroutine(ProcessCardUse(currentCards[slotIndex]));
        return true;
    
    }

    private IEnumerator ProcessCardUse(ActiveCardInfo usedCard) {

        CardStateManager.Instance.SetUsingState(usedCard.cardInstance, true);

        //todo
        // 1. 播放使用动画
        // 2. 应用卡牌效果
        // 3. 清除卡牌实例
        yield return new WaitForSeconds(0.2f);
        
        CardStateManager.Instance.SetUsingState(usedCard.cardInstance, false);
        ClearSlot(cardSlots[System.Array.IndexOf(currentCards, usedCard)]);

    }

    private void ClearAllSlots() {
    
        foreach(Transform slot in cardSlots) {
        
            ClearSlot(slot);
        
        }
    }

    private void ClearSlot(Transform slot) {
        
        int slotIndex = System.Array.IndexOf(cardSlots, slot);

        if (slotIndex != -1) {

            foreach(Transform child in slot) {
            
                Destroy(child.gameObject);
            
            }
        
        }
    }

    public void SetCardQueueDraggingState(bool isAnyCardDragging) {

        IsAnyCardDragging = isAnyCardDragging;

    }

    [Header("基础配置")]
    public GameObject cardPrefab;

}