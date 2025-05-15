using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class TipManager : MonoBehaviour {
    public static TipManager Instance { get; private set; }

    [Header("UI 组件")]
    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("动画参数")]
    [SerializeField] private float fadeDuration = 0.5f; // 淡入淡出时间
    [SerializeField] private float defaultDisplayTime = 2f; // 默认显示时间

    [Header("Boss台词")]
    [SerializeField]
    public List<string> BossTips = new List<string> {

        "我还会回来的",
        "事不过三…",
        "你赢了…",
        "又是你？！",
        "这不可能…",
        "你是个魔鬼",
        "你到底有什么目的？",
        "我受够了",
        "哈哈哈哈哈，你个作弊者",
        "你赢了，但是我妈叫我回家吃饭了"

    };

    private Tween currentTween;

    private GameObject birthRoom;

    private void Awake() {

        if (Instance != null && Instance != this) {

            Destroy(gameObject);
            return;

        }

        Instance = this;
        DontDestroyOnLoad(this);

        canvasGroup.alpha = 0f;

    }

    void Start() {

        if (GameDataManager.Instance.CompleteCnt == 0)
            StartCoroutine(PlayInitialTips());
            
    }

    void Update() {

        if (birthRoom == null)
            birthRoom = GameObject.FindGameObjectWithTag("BirthRoom");

    }

    public void ShowTip(string message, float duration = -1f) {

        if (duration <= 0f)
            duration = defaultDisplayTime;

        // 终止前一个提示动画
        if (currentTween != null && currentTween.IsActive()) {
            currentTween.Kill();
        }

        tipText.text = message;
        canvasGroup.alpha = 0f;

        currentTween = DOTween.Sequence()
            .Append(canvasGroup.DOFade(1f, fadeDuration))
            .AppendInterval(duration)
            .Append(canvasGroup.DOFade(0f, fadeDuration))
            .OnComplete(() => currentTween = null);

    }

    private IEnumerator PlayInitialTips() {

        yield return new WaitForSeconds(2f);
        ShowTip("心魔既出，不破不立", 3f);

        yield return new WaitForSeconds(3f + 2f);
        ShowTip("万机千变，皆由心控", 3f);

    }

}
