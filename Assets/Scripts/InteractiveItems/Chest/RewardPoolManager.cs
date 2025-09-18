using UnityEngine;
using System.Collections.Generic;

public class ChestRewardInfo {

    public ChestRewardInfo(CardDataBase cd, bool isp) {

        card = cd;
        isSpecial = isp;

    }

    public CardDataBase card;
    public bool isSpecial; // 是否是雷电宝箱

}

public class RewardPoolManager : MonoBehaviour {

    [Header("普通卡牌池")]
    public List<CardDataBase> commonCardList = new();

    [Header("生成的奖励")]
    public List<ChestRewardInfo> chestRewards = new();

    [Header("敌人房间数量")]
    [SerializeField] private int enemyRoomCount = 4;
    public int curRoomIndex = 0;

    [Header("雷电卡牌ID")]
    [SerializeField] private int lightningCardID = 5;

    [Header("雷电卡牌")]
    [SerializeField] private CardDataBase lightningCard;

    public static RewardPoolManager Instance { get; private set; }

    void Awake() {

        if (Instance != null && Instance != this) {

            Destroy(gameObject);
            return;

        }

        Instance = this;

    }
    void Start() {

        GenerateChestRewards();

    }

    public void GenerateChestRewards() {

        chestRewards.Clear();
        int chestCount = enemyRoomCount + 1;

        var cardPoolManager = CardPoolManager.Instance;

        commonCardList = cardPoolManager.GetOwnedCardsFromAllPoolsExceptForShieldCard();

        // 先随机生成普通奖励
        for (int i = 0; i < chestCount; i++) {

            var card = GetRandomCard();
            chestRewards.Add(new ChestRewardInfo(card, false));

        }

        // 随机选一个宝箱作为特殊奖励
        int specialIndex = Random.Range(0, chestRewards.Count);

        var CurrentlightningCard = cardPoolManager.FindCardInPools(lightningCardID);

        if (CurrentlightningCard == null)
            CurrentlightningCard = Instantiate(lightningCard);

        var specialChestRewardInfo = new ChestRewardInfo(CurrentlightningCard, true);

        chestRewards[specialIndex] = specialChestRewardInfo;

    }

    private CardDataBase GetRandomCard() {

        return commonCardList[Random.Range(0, commonCardList.Count)];

    }

    public ChestRewardInfo GetRewardForChest(int chestIndex) {

        return chestRewards[chestIndex];

    }

}
