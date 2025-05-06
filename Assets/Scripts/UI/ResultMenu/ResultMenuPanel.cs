using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ResultMenuPanel : MonoBehaviour {

    [Header("提示词配置")]
    [SerializeField] private TextMeshProUGUI prompt1;
    [SerializeField] private TextMeshProUGUI prompt2;
    [SerializeField] private TextMeshProUGUI cardUsedText;
    [SerializeField] private TextMeshProUGUI enemyKilledText;
    [SerializeField] private TextMeshProUGUI damageCausedText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI levelText;

    private List<TextMeshProUGUI> prompts1;

    [Header("页面配置")]
    [SerializeField] private GameObject page2;

    [Header("持续时间配置")]
    [SerializeField] private float promptInterval = 0.75f;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("按钮配置")]
    [SerializeField] private Button infinityButton;
    [SerializeField] private Button resetButton;

    private bool prompts1Completed;

    void Start() {

        var runStatTracker = RunStatTracker.Instance;

        cardUsedText.text = $"卡牌使用次数： {runStatTracker.TotalCardsUsed}";
        enemyKilledText.text = $"敌人击败次数： {runStatTracker.TotalEnemiesKilled}";
        damageCausedText.text = $"造成的总伤害： {runStatTracker.TotalDamageAmount}";

        timerText.text = runStatTracker.IntToTotalTime();
        levelText.text = runStatTracker.HandleCompleteLevelText();

        prompts1Completed = false;

        prompts1 = new List<TextMeshProUGUI> {

            prompt1,
            prompt2,
            cardUsedText,
            enemyKilledText,
            damageCausedText,
            timerText,

        };

        infinityButton.onClick.AddListener(OnInfinityClicked);
        resetButton.onClick.AddListener(OnResetClicked);

        StartCoroutine(ShowCompleteDataCoroutine());

    }

    void Update() {

        if (prompts1Completed && Input.GetMouseButtonDown(0)) {

            StartCoroutine(FadeOutPrompts1AndFadeInPage2());

        }

    }

    private IEnumerator ShowCompleteDataCoroutine() {

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < prompts1.Count; i++) {


            UIFadeHelper.TextFadeIn(prompts1[i]);

            yield return new WaitForSeconds(promptInterval);

        }

        UIFadeHelper.TextFadeIn(levelText);
        prompts1Completed = true;

    }

    private IEnumerator FadeOutPrompts1AndFadeInPage2() {

        FadeOutPrompts1();

        yield return new WaitForSeconds(2f);

        FadeInPage2();


    }

    private void FadeOutPrompts1() {

        for (int i = 0; i < prompts1.Count; i++) {

            UIFadeHelper.TextFadeOut(prompts1[i]);

        }

    }

    private void FadeInPage2() {

        page2.SetActive(true);

        var group = page2.GetComponent<CanvasGroup>() ?? page2.AddComponent<CanvasGroup>();

        group.alpha = 0;
        group.interactable = false;

        group.DOFade(1f, fadeDuration).OnComplete(() => {

            group.interactable = true;

        });

    }

    private void OnInfinityClicked() {

        SceneTransitionHelper.Instance.LoadSceneWithTransition("LevelScene");
        BGMManager.Instance.PlayBGM(BGMManager.Instance.normalBGM);

    }

    private void OnResetClicked() {

        RunStatTracker.Instance.ResetCurrentStats();
        RunStatTracker.Instance.ResetTotalStats();
        GameDataManager.Instance.ResetCompleteCnt();
        SceneTransitionManager.Instance.IsCompleteSceneLoaded = false;

        SceneTransitionHelper.Instance.LoadSceneWithTransition("LevelScene");
        BGMManager.Instance.PlayBGM(BGMManager.Instance.normalBGM);

    }

}
