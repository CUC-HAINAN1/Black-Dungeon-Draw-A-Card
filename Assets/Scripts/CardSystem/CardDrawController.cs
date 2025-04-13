using Google.Events.Protobuf.Cloud.Datastore.V1;

using UnityEngine;

public class CardDrawController : MonoBehaviour {

    public CardAnimator cardAnimator;
    private CardQueueSystem cardQueueSystem;

    [Header("自动抽卡参数")]
    public float autoDrawInterval = 1f;
    private float drawTimer;


    private void Start() {

        cardAnimator = GetComponent<CardAnimator>();
        cardQueueSystem = CardQueueSystem.Instance;
        cardQueueSystem.Initialize();

    }

    private void Update() {

        if (!PlayerAttributes.Instance || PlayerAttributes.Instance.IsDead)
            return;

        drawTimer += Time.deltaTime;

        if (drawTimer >= autoDrawInterval) {

            DrawAndCreateCard(10);
            drawTimer = 0f;

        }

        for (int i = 1; i <= 7; i++) {

            if (Input.GetKeyDown(KeyCode.Alpha0 + i)) {

                DrawAndCreateCard(i);

            }

        }

    }

    private void DrawAndCreateCard(int num)
    {
        Transform targetSlot = cardQueueSystem.GetNextSlot();
        if (targetSlot == null) {

            Debug.Log("没有可用卡槽");
            return;
        }

        if (!TryCreateCard(targetSlot, out var newCardInfo, num)) {

            Debug.LogError("卡牌创建失败");
            return;
        }

        PlayCardEntranceAnimation(newCardInfo);

    }

    private void PlayCardEntranceAnimation(CardQueueSystem.ActiveCardInfo cardInfo) {

        // 空引用保护
        if (cardInfo?.cardInstance == null) return;

        cardAnimator.PlayCardEntrance(

                cardInfo.cardInstance,
                cardQueueSystem.cardSlots[System.Array.IndexOf(cardQueueSystem.currentCards, cardInfo)]

        );

    }

    private bool TryCreateCard(Transform targetSlot, out CardQueueSystem.ActiveCardInfo cardInfo, int num) {

        CardDataBase cardData = null;

        // 抽卡逻辑
        if (num == 10) {

            cardData = cardQueueSystem.DrawCard();

        } else {

            //根据键盘输入编号抽卡
            foreach (var card in CardPoolManager.Instance.allCards) {

                if (num == card.cardID) {

                    cardData = card;

                }

            }

        }

        // 创建卡牌
        cardQueueSystem.CreateCard(cardData, targetSlot);

        // 获取新卡信息
        int slotIndex = System.Array.IndexOf(cardQueueSystem.cardSlots, targetSlot);
        cardInfo = cardQueueSystem.currentCards[slotIndex];

        // 初始化视觉组件
        CardVisual visual = cardInfo.cardInstance.GetComponent<CardVisual>();
        visual.Initialize(cardData);

        return true;

    }

}
