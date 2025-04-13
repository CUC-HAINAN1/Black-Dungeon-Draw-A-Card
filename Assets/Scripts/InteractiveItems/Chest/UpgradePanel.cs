using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUIPanel : MonoBehaviour {

    [Header("UI 元素")]
    public Image iconImage;
    public TMP_Text descriptionText;
    public Button confirmButton;

    public void Setup(CardDataBase card, CardDataBase.UpgradableParam param, System.Action onConfirm = null) {

        iconImage.sprite = card.cardIcon;

        descriptionText.text = $"{param.displayName}";

        confirmButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(() => {
            onConfirm?.Invoke();
            Destroy(gameObject);
        });

    }
}
