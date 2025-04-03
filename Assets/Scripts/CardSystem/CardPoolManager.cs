using UnityEngine;
using System.Collections.Generic;

public class CardPoolManager : MonoBehaviour {
    public static CardPoolManager Instance { get; private set; }

    [Header("卡牌数据源")]
    public List<CardDataBase> allCards = new();

    [Header("卡池信息配置")]

    [SerializeField] public int RadePollDrawInterval = 5;

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
        InitializePools();

        Debug.Log("卡池管理器初始化");

    }

    public void InitializePools() {

        Debug.Log($"总卡牌数: {allCards.Count}");
        commonPool.Clear();
        rarePool.Clear();

        foreach (var card in allCards) {
            
            if (!card.Owned) continue;

            switch (card.rarity) {
                case CardDataBase.Rarity.Common:

                    commonPool.Add(card);
                    break;

                case CardDataBase.Rarity.Rare:

                    rarePool.Add(card);
                    break;

            }
        }

        Debug.Log($"普通卡池初始化数量: {commonPool.Count}");
        Debug.Log($"稀有卡池初始化数量: {rarePool.Count}");

    }

    // 公开的抽卡方法
    public CardDataBase DrawCard() {

        drawCounter++;
        bool isRareDraw = drawCounter % RadePollDrawInterval == 0;

        List<CardDataBase> targetPool = isRareDraw ? rarePool : commonPool;

        if (targetPool.Count == 0) {

            Debug.Log($"目标卡池为空，强制重置");
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

        if (!requireRare) return commonPool;

        return rarePool.Count > 0 ? rarePool : commonPool;

    }

    //玩家获得新卡
    public void AddCardToPool(CardDataBase newCard) {

        if (!newCard.Owned) return;

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
}