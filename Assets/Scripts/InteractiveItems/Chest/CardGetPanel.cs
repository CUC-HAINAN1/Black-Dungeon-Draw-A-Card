using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardGetPanelUIPanel : MonoBehaviour {

    [Header("UI 元素")]
    public GameObject cardPrefab;
    public TMP_Text descriptionText;
    public Button confirmButton;

    public void Setup(CardDataBase card, System.Action onConfirm = null) {

        var cardInstance = Instantiate(cardPrefab, cardPrefab.transform.parent);

        cardInstance.GetComponent<CardVisual>().Initialize(card);

        var dragHandler = cardInstance.GetComponent<CardDragHandler>();

        if (dragHandler != null) {

            dragHandler.DestroyThis();
            dragHandler.enabled = false;

        }
        descriptionText.text = "奇卷现世，机缘难再也";

        confirmButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(() => {
            onConfirm?.Invoke();
            Destroy(gameObject);
        });

    }
}
