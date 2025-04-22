using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;

public class TipManager : MonoBehaviour {
    public static TipManager Instance { get; private set; }

    [Header("UI 组件")]
    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("动画参数")]
    [SerializeField] private float fadeDuration = 0.5f; // 淡入淡出时间
    [SerializeField] private float defaultDisplayTime = 2f; // 默认显示时间

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
        ShowTip("WASD 移动，Shift 翻滚", 3f);

        yield return new WaitForSeconds(3f + 2f);
        ShowTip("拖拽卡牌以释放技能", 3f);

    }

}
