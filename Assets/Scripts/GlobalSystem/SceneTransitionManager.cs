using System.Collections;
using UnityEngine;

public class SceneTransitionManager : MonoBehaviour {

    [Header("配置参数")]
    public float delayBeforeSceneLoad = 2f;
    private bool hasTriggeredSceneLoad = false;
    public CardDataBase lightningCard;
    public BossData runtimeBossDB;

    public bool IsCompleteSceneLoaded { get; set; }

    public static SceneTransitionManager Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {

            Destroy(gameObject);
            return;

        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    private void OnEnable() {

        EventManager.Instance.Subscribe("PlayerDied", OnPlayerDied);
        EventManager.Instance.Subscribe("BossDied", OnBossDied);

    }

     private void OnDisable() {

        if (EventManager.Instance != null) {

            EventManager.Instance.Unsubscribe("PlayerDied", OnPlayerDied);
            EventManager.Instance.Unsubscribe("BossDied", OnBossDied);

        }
    }

    private void OnPlayerDied(object eventData) {

        TriggerSceneSwitch("DefeatScene", () => {
            CustomLogger.Log("玩家死亡 - 即将切换到死亡场景");

        });

        BGMManager.Instance.PlayBGM(BGMManager.Instance.menuBGM);

    }

    private void OnBossDied(object eventData) {

        if (GameDataManager.Instance.CompleteCnt == 3 && !IsCompleteSceneLoaded) {

            IsCompleteSceneLoaded = true;

            TriggerSceneSwitch("ResultScene", () => {
                CustomLogger.Log("通关三次 - 即将切换到结算场景");

            });

        }
        else {

            TriggerSceneSwitch("CompleteScene", () => {
                CustomLogger.Log("Boss 死亡 - 即将切换到通关场景");

            });

        }

        BGMManager.Instance.PlayBGM(BGMManager.Instance.menuBGM);

    }

    private void TriggerSceneSwitch(string sceneName, System.Action beforeLoadAction = null) {

        if (hasTriggeredSceneLoad)
            return;

        hasTriggeredSceneLoad = true;
        StartCoroutine(DelayThenLoadScene(sceneName, beforeLoadAction));

    }

    private IEnumerator DelayThenLoadScene(string sceneName, System.Action beforeLoadAction) {

        yield return new WaitForSeconds(delayBeforeSceneLoad);

        beforeLoadAction?.Invoke();

        hasTriggeredSceneLoad = false;

        SceneTransitionHelper.Instance.LoadSceneWithTransition(sceneName);

    }
}
