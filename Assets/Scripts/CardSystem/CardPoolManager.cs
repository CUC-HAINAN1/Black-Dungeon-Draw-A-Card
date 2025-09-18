using UnityEngine;
using System.Collections.Generic;

public class CardPoolManager : MonoBehaviour {
    public static CardPoolManager Instance { get; private set; }

    [Header("卡牌数据源")]
    public List<CardDataBase> allCards = new();

    [Header("卡池信息配置")]

    [SerializeField] public int RadePollDrawInterval = 5;
    private Dictionary<CardDataBase, CardDataBase> runtimeCardMap = new(); //克隆卡牌数据源，防止影响本地数据
    // 运行时卡池
    private List<CardDataBase> commonPool = new();
    private List<CardDataBase> rarePool = new();

    // 抽卡计数器
    private int drawCounter;

    private void Awake() {

        if (Instance != null) {

            Destroy(gameObject);
            return;

        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // --- 在这里添加重置代码 ---
        // 在初始化卡池之前，强制重置特定卡牌的状态
        ResetSpecificCardStates();

        InitializePools();

    }

    public void InitializePools() {

        commonPool.Clear();
        rarePool.Clear();
        runtimeCardMap.Clear();

        foreach (var card in allCards) {

            // 克隆一份，不污染原始数据
            CardDataBase clone = Instantiate(card);
            runtimeCardMap[card] = clone;

            if (!card.Owned)
                continue;

            switch (card.rarity) {
                case CardDataBase.Rarity.Common:

                    commonPool.Add(runtimeCardMap[card]);
                    break;

                case CardDataBase.Rarity.Rare:

                    rarePool.Add(runtimeCardMap[card]);
                    break;

            }
        }

    }

    // 抽卡方法
    public CardDataBase DrawCard() {

        drawCounter++;
        bool isRareDraw = drawCounter % RadePollDrawInterval == 0;

        List<CardDataBase> targetPool = isRareDraw ? rarePool : commonPool;

        if (targetPool.Count == 0) {

            targetPool = GetFallbackPool(isRareDraw);

        }

        return GetRandomCard(targetPool);

    }

    // 获取随机卡牌
    private CardDataBase GetRandomCard(List<CardDataBase> pool) {

        int index = Random.Range(0, pool.Count);

        return pool[index];

    }

    // 后备卡池策略(若稀有卡池无卡则降级为普通卡池)
    private List<CardDataBase> GetFallbackPool(bool requireRare) {

        if (!requireRare)
            return commonPool;

        return rarePool.Count > 0 ? rarePool : commonPool;

    }

    //玩家获得新卡
    public void AddCardToPool(CardDataBase newCard) {

        if (!newCard.Owned)
            return;

        switch (newCard.rarity) {

            case CardDataBase.Rarity.Common:

                if (!commonPool.Contains(newCard))
                    commonPool.Add(newCard);

                break;

            case CardDataBase.Rarity.Rare:

                if (!rarePool.Contains(newCard))
                    rarePool.Add(newCard);

                break;

        }
    }

    //找到某张卡牌
    public CardDataBase FindCardInPools(int id) {

        foreach (var card in commonPool) {

            if (card.cardID == id)
                return card;

        }

        foreach (var card in rarePool) {

            if (card.cardID == id)
                return card;

        }

        CustomLogger.Log($"未在卡池中找到 ID 为 {id} 的卡牌");
        return null;

    }

    //获得除了护盾卡之外的所有卡
    public List<CardDataBase> GetOwnedCardsFromAllPoolsExceptForShieldCard() {

        List<CardDataBase> result = new();

        foreach (var card in commonPool) {

            //假如遇到护盾卡就跳过
            if (card.cardID == 7)
                continue;

            if (card.Owned)
                result.Add(card);
        }

        foreach (var card in rarePool)
            if (card.Owned)
                result.Add(card);

        return result;

    }

    public List<CardDataBase> GetOwnedCardsFromAllPools() {

        List<CardDataBase> result = new();

        foreach (var card in commonPool) {
            if (card.Owned)
                result.Add(card);
        }

        foreach (var card in rarePool)
            if (card.Owned)
                result.Add(card);

        return result;

    }


    /// <summary>
    /// 新增方法：用于在游戏启动时重置特定卡牌的状态。
    /// </summary>
    private void ResetSpecificCardStates() {
        // 找到ID为5的卡牌
        CardDataBase hiddenCard = allCards.Find(card => card.cardID == 5);

        // 如果找到了这张卡
        if (hiddenCard != null) {
            // 无论它在资产文件中是什么状态，都在游戏开始时强制设为“未拥有”
            hiddenCard.Owned = false;
            Debug.Log($"卡牌 '{hiddenCard.displayName}' (ID: 5) 的状态已在启动时重置为 '未拥有'.");
        }
        else {
            Debug.LogWarning("在 allCards 列表中未找到 ID 为 5 的卡牌来进行状态重置。");
        }


    }
}
