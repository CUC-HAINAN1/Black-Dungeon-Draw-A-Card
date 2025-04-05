using UnityEngine;

public class CardDrawController : MonoBehaviour {

    public CardAnimator _cardAnimator;
    private CardQueueSystem _cardSystem;

    private void Start() {
    
        _cardAnimator = GetComponent<CardAnimator>();
        _cardSystem = CardQueueSystem.Instance;
        CardQueueSystem.Instance.Initialize();
    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DrawAndCreateCard();
        }
    }

    private void DrawAndCreateCard()
    {
        Transform targetSlot = _cardSystem.GetNextSlot();
        if (targetSlot == null) {
        
            Debug.LogWarning("没有可用卡槽");
            return;
        }

        if (!TryCreateCard(targetSlot, out var newCardInfo)) {
        
            Debug.LogError("卡牌创建失败");
            return;
        }

        PlayCardEntranceAnimation(newCardInfo);
    
    }

    private void PlayCardEntranceAnimation(CardQueueSystem.ActiveCardInfo cardInfo) {
    
        // 空引用保护
        if (cardInfo?.cardInstance == null) return;

        _cardAnimator.PlayCardEntrance(
            
                cardInfo.cardInstance, 
                _cardSystem.cardSlots[System.Array.IndexOf(_cardSystem.currentCards, cardInfo)]
            
        );

        // 更新状态管理器
        CardStateManager.Instance.RegisterCard(cardInfo.cardInstance);
    }

    private bool TryCreateCard(Transform targetSlot, out CardQueueSystem.ActiveCardInfo cardInfo) {
    
        cardInfo = null;
        
        // 抽卡逻辑
        CardDataBase cardData = _cardSystem.DrawCard();

        // 创建卡牌
        _cardSystem.CreateCard(cardData, targetSlot);
        
        // 获取新卡信息
        int slotIndex = System.Array.IndexOf(_cardSystem.cardSlots, targetSlot);
        cardInfo = _cardSystem.currentCards[slotIndex];
        
        // 初始化视觉组件
        CardVisual visual = cardInfo.cardInstance.GetComponent<CardVisual>();
        visual.Initialize(cardData);

        return true;
    }

}