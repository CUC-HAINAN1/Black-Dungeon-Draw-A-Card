using UnityEngine;

[RequireComponent(typeof(CardQueueSystem))]
[RequireComponent(typeof(CardAnimator))]
public class CardDrawController : MonoBehaviour
{
    public CardQueueSystem queueSystem;
    public CardAnimator cardAnimator;

    private void Start()
    {
        queueSystem = GetComponent<CardQueueSystem>();
        cardAnimator = GetComponent<CardAnimator>();
        queueSystem.Initialize();
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
        CardDataBase card = queueSystem.DrawCard();
        
            Transform targetSlot = queueSystem.GetNextSlot();
            
            queueSystem.CreateCard(card, targetSlot);
            
            int slotIndex = System.Array.IndexOf(queueSystem.cardSlots, targetSlot);
        
            CardQueueSystem.ActiveCardInfo newCardInfo = queueSystem.currentCards[slotIndex];
            CardVisual visual = newCardInfo.cardInstance.GetComponent<CardVisual>();
                
            visual.Initialize(card);
            cardAnimator.PlayCardEntrance(newCardInfo.cardInstance, targetSlot);
                
    }

}