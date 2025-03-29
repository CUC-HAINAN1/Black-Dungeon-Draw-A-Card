using UnityEngine;

public class CardDrawController : MonoBehaviour
{
    [Header("卡牌位置配置")]
    public Transform[] cardSlots = new Transform[4];

    [Header("卡牌预制件")] 
    public GameObject cardPrefab;

    //当前应该填充的卡牌位置索引
    private int currentSlotIndex = 0;

    //抽取到的卡牌的可视化与保存
    [System.Serializable]
    public class ActiveCardInfo
    {
        public CardDataBase cardData;
        public GameObject cardInstance;
    }

    [Header("当前卡牌状态")]
    [SerializeField] public ActiveCardInfo[] currentCards = new ActiveCardInfo[4];


    private void Start() {
    
        ClearAllSlots();

    }

    private void Update() {
    
        if (Input.GetKeyDown(KeyCode.Space)) {
        
            DrawAndCreateCard();
        }
    
    }

    private void DrawAndCreateCard() {

        var drawnCard = CardPoolManager.Instance.DrawCard();
        
        if (drawnCard != null) {

            CreateCardVisual(drawnCard);
            Debug.Log($"抽到卡牌：{drawnCard.displayName}");
        
        }
    
    }

    private void CreateCardVisual(CardDataBase data) {
        
        Transform targetSlot = cardSlots[currentSlotIndex];

        ClearSlot(targetSlot);

        GameObject newCard = Instantiate(cardPrefab, 
                
                targetSlot.position, 
                targetSlot.rotation,
                targetSlot
        
        );

        RectTransform rt = newCard.GetComponent<RectTransform>();
        
        rt.localPosition = Vector3.zero;
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one;

        CardVisual visual = newCard.GetComponent<CardVisual>();
        visual.Initialize(data);

        currentSlotIndex = (currentSlotIndex + 1) % cardSlots.Length;

        //已抽到的卡牌数据存储
        currentCards[currentSlotIndex] = new ActiveCardInfo {
    
                cardData = data,
                cardInstance = newCard
        
        };

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

}