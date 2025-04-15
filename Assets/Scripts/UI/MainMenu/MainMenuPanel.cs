using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuPanel : MonoBehaviour {

    [Header("UI 按钮")]
    public Button startButton;
    public Button settingButton;
    public Button exitButton;

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

        Debug.Log("开始游戏！");
        SceneTransitionHelper.Instance.LoadSceneWithTransition("SangTestScene");

    }

    private void OnSettingClicked() {

        Debug.Log("打开设置");
        if (settingPanelInstance != null)
            settingPanelInstance.SetActive(true);

    }

    private void OnExitClicked() {

        Debug.Log("退出游戏！");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }
}
