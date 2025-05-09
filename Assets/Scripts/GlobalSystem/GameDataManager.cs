using UnityEngine;
using System.Linq;

public class GameDataManager : MonoBehaviour {
    public CardDataBase[] originalCardsDB;
    public CardDataBase[] runtimeCardsDB;

    public EnemyData originalEnemyDB;
    public EnemyData runtimeEnemyDB;

    public BossData originalBossDB;
    public BossData runtimeBossDB;

    public int CompleteCnt { get; private set; }

    public static GameDataManager Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {

            Destroy(gameObject);
            return;

        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CompleteCnt = 0;

    }

    private void OnEnable() {

        EventManager.Instance.Subscribe("BossDied", OnBossDied);
        EventManager.Instance.Subscribe("PlayerDied", OnPlayerDied);

    }

    private void OnDisable() {

        EventManager.Instance.Unsubscribe("BossDied", OnBossDied);
        EventManager.Instance.Unsubscribe("PlayerDied", OnPlayerDied);

    }

    private void OnPlayerDied(object eventData) {

        ResetRuntimeData();
        ResetCompleteCnt();

        RunStatTracker.Instance.StopTracking();
        RunStatTracker.Instance.ResetTotalStats();
        RunStatTracker.Instance.ResetCurrentStats();

    }

    private void OnBossDied(object eventData) {

        ApplyBossUpgrade();
        ApplyEnemyUpgrade();
        ApplyCardsUpgrade();

        HandleCompleteCnt();

        RunStatTracker.Instance.StopTracking();
        RunStatTracker.Instance.ApplyTotalStats();
        RunStatTracker.Instance.ResetCurrentStats();

    }

    public void HandleCompleteCnt() {

        CompleteCnt++;

    }

    private void ResetRuntimeData() {

        for (int i = 0; i < runtimeCardsDB.Length; i++) {

            var origin = originalCardsDB.FirstOrDefault(c => c.cardID == runtimeCardsDB[i].cardID);

            if (origin != null)
                CardDataBase.CopyUpgradeFields(origin, runtimeCardsDB[i], origin.cardID);

        }

        EnemyData.CopyEnemyData(originalEnemyDB, runtimeEnemyDB);
        BossData.CopyBossData(originalBossDB, runtimeBossDB);
    }

    private void ApplyBossUpgrade() {

        if (runtimeBossDB.skillCooldown >= 1) {

            runtimeBossDB.skillCooldown -= 0.2f;

        }
        if (runtimeBossDB.moveSpeed <= 20) {

            runtimeBossDB.moveSpeed += 2;

        }

        runtimeBossDB.maxHealth += 500;

    }

    private void ApplyEnemyUpgrade() {

        runtimeEnemyDB.maxHealth += 20;

    }

    private void ApplyCardsUpgrade() {

        for (int i = 0; i < runtimeCardsDB.Length; i++) {

            var upgradedCard = CardPoolManager.Instance.FindCardInPools(runtimeCardsDB[i].cardID);

            if (upgradedCard != null) {

                CardDataBase.CopyUpgradeFields(upgradedCard, runtimeCardsDB[i], upgradedCard.cardID);

            }
        }

    }

    public void ResetCompleteCnt() {

        CompleteCnt = 0;

    }

}
