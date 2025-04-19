using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathMenuPanel : MonoBehaviour {

    [Header("UI 按钮")]
    public Button TryAgainButton;
    public Button ReturnButton;

    private void Start() {

        // 按钮绑定事件
        TryAgainButton.onClick.AddListener(OnStartClicked);
        ReturnButton.onClick.AddListener(OnReturnClicked);

    }

    private void OnStartClicked() {

        CustomLogger.Log("再次开始游戏！");
        SceneTransitionHelper.Instance.LoadSceneWithTransition("LevelScene");

    }

    private void OnReturnClicked() {

        CustomLogger.Log("回到主菜单！");
        SceneTransitionHelper.Instance.LoadSceneWithTransition("MainMenuScene");

    }
}
