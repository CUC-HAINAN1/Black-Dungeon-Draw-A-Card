using UnityEngine;

public class CardDrawController : MonoBehaviour {

    public CardAnimator _cardAnimator;
    private CardQueueSystem _cardQueueSystem;
    private CardStateManager _cardStateManger;

    private void Start() {
    
        _cardAnimator = GetComponent<CardAnimator>();
        _cardQueueSystem = CardQueueSystem.Instance;
        _cardStateManger = CardStateManager.Instance;
        _cardQueueSystem.Initialize();
    
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
        Transform targetSlot = _cardQueueSystem.GetNextSlot();
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
                _cardQueueSystem.cardSlots[System.Array.IndexOf(_cardQueueSystem.currentCards, cardInfo)]
            
        );

    }

    private bool TryCreateCard(Transform targetSlot, out CardQueueSystem.ActiveCardInfo cardInfo) {
    
        cardInfo = null;
        
        // 抽卡逻辑
        CardDataBase cardData = _cardQueueSystem.DrawCard();

        // 创建卡牌
        _cardQueueSystem.CreateCard(cardData, targetSlot);
        
        // 获取新卡信息
        int slotIndex = System.Array.IndexOf(_cardQueueSystem.cardSlots, targetSlot);
        cardInfo = _cardQueueSystem.currentCards[slotIndex];
        
        // 初始化视觉组件
        CardVisual visual = cardInfo.cardInstance.GetComponent<CardVisual>();
        visual.Initialize(cardData);

        return true;
    }

}