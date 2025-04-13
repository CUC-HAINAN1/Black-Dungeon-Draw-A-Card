using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RewardUIManager : MonoBehaviour {

    public static RewardUIManager Instance { get; private set; }

    [Header("预制体引用")]
    public GameObject upgradeUIPrefab;
    public GameObject newCardUIPrefab;

    [Header("UI 容器")]
    public Transform rewardUIContainer;

    private void Awake() {

        if (Instance != null) {

            Destroy(gameObject);
            return;

        }

        Instance = this;

    }

    // 展示强化 UI
    public void ShowUpgradeUI(CardDataBase card, CardDataBase.UpgradableParam param) {


        GameObject ui = Instantiate(upgradeUIPrefab, rewardUIContainer);

        UpgradeUIPanel panel = ui.GetComponent<UpgradeUIPanel>();

        panel.Setup(card, param);

    }

    // 展示获得新卡 UI
    public void ShowNewCardUI(CardDataBase card) {

        GameObject ui = Instantiate(newCardUIPrefab, rewardUIContainer);

        CardGetPanelUIPanel panel = ui.GetComponent<CardGetPanelUIPanel>();

        panel.Setup(card);

    }

}
