using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public static class UIAnimUtils {

    public static void PopIn(RectTransform target, float duration = 0.35f, float fadeFrom = 0f, float fadeTo = 1f) {

        // Scale-in 动画
        target.localScale = Vector3.zero;
        target.DOScale(Vector3.one, duration).SetEase(Ease.OutBack);

        // Alpha 淡入动画
        CanvasGroup cg = target.GetComponent<CanvasGroup>();

        if (cg == null) {
            cg = target.gameObject.AddComponent<CanvasGroup>();
        }

        cg.alpha = fadeFrom;
        cg.DOFade(fadeTo, duration).SetEase(Ease.InOutQuad);
    }
}
