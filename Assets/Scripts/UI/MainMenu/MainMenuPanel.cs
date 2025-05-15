using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuPanel : MonoBehaviour {

    [Header("文本")]
    public GameObject title;
    public GameObject nameList;

    [Header("UI 按钮")]
    public Button startButton;
    public Button settingButton;
    public Button exitButton;

    public GameObject startButtonGameObject;
    public GameObject exitButtonGameObject;

    [Header("面板控制")]
    public GameObject settingPanel;
    private GameObject settingPanelInstance;

    private void Start() {

        // 按钮绑定事件
        startButton.onClick.AddListener(OnStartClicked);
        settingButton.onClick.AddListener(OnSettingClicked);
        exitButton.onClick.AddListener(OnExitClicked);

        Canvas parentCanvas = GetComponentInParent<Canvas>();

        // 初始设置界面隐藏
        settingPanelInstance = Instantiate(settingPanel, parentCanvas.transform, false);

        settingPanelInstance.SetActive(false);

    }

    private void OnStartClicked() {

        CustomLogger.Log("开始游戏！");
        BGMManager.Instance.PlayBGM(BGMManager.Instance.normalBGM);
        SceneTransitionHelper.Instance.LoadSceneWithTransition("LevelScene");

    }

    private void OnSettingClicked() {

        CustomLogger.Log("打开设置");

        if (settingPanelInstance.activeSelf == false)
            settingPanelInstance.SetActive(true);
        else
            settingPanelInstance.SetActive(false);

        ReverseOtherComponents();

    }

    private void ReverseOtherComponents() {

        startButtonGameObject.SetActive(!startButtonGameObject.activeSelf);
        exitButtonGameObject.SetActive(!exitButtonGameObject.activeSelf);
        title.SetActive(!title.activeSelf);
        nameList.SetActive(!nameList.activeSelf);

    }

    private void OnExitClicked() {

        CustomLogger.Log("退出游戏！");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
}
