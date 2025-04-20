using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour {

    [SerializeField] private Button returnButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button returnMenuButton;

    void Start() {

        Time.timeScale = 0;

        returnButton.onClick.AddListener(OnclickReturnButton);
        restartButton.onClick.AddListener(OnclickRestartButton);
        returnMenuButton.onClick.AddListener(OnclickReturnMenuButton);

    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {

            ResumeGame();
            Destroy(gameObject);

        }

    }

    void OnclickReturnButton() {

        ResumeGame();
        Destroy(gameObject);

    }

    void OnclickRestartButton() {

        ResumeGame();
        SceneTransitionHelper.Instance.LoadSceneWithTransition("LevelScene");

    }

    void OnclickReturnMenuButton() {

        ResumeGame();
        SceneTransitionHelper.Instance.LoadSceneWithTransition("MainMenuScene");


    }

    void ResumeGame() {

        Time.timeScale = 1;
        GameRunningUIManager.Instance.DestroyPauseUI();

    }

}
