using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
public static class UIFadeHelper {

    public static void TextFadeIn(TextMeshProUGUI text, float duration = 0.5f) {

        if (!text.gameObject.activeSelf)
            text.gameObject.SetActive(true);

        text.enabled = true;
        text.alpha = 0;

        text.DOFade(1f, duration);

    }

    public static void TextFadeOut(TextMeshProUGUI text, float duration = 0.5f) {

        text.DOFade(0f, duration);

    }


}


