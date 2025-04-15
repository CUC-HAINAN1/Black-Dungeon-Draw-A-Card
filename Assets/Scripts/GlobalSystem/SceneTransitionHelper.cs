using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionHelper : MonoBehaviour {

    public Canvas persistentCanvasPrefab;
    private Canvas persistentCanvasInstance;
    private Image transitionImage;
    public float fadeDuration = 1f;

    public static SceneTransitionHelper Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {

            Destroy(gameObject);
            return;

        }

        else {

            Instance = this;
            DontDestroyOnLoad(this);

            // 动态实例化 PersistentCanvas
            persistentCanvasInstance = Instantiate(persistentCanvasPrefab);
            DontDestroyOnLoad(persistentCanvasInstance.gameObject);

            transitionImage = persistentCanvasInstance.GetComponentInChildren<Image>(true);

        }
    }

    private void Start() {

        // 初始黑幕置透明
        if (transitionImage != null) {

            Color c = transitionImage.color;
            c.a = 0f;
            transitionImage.color = c;

        }
    }

    //从按钮调用：开始加载目标场景
    public void LoadSceneWithTransition(string sceneName) {

        StartCoroutine(DoTransition(sceneName));

    }

    private IEnumerator DoTransition(string sceneName) {
        //淡出
        yield return StartCoroutine(FadeOut());

        //加载新场景
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    //新场景加载完毕后，自动淡入
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {

        SceneManager.sceneLoaded -= OnSceneLoaded;
        StartCoroutine(FadeIn());

    }

    private IEnumerator FadeOut() {

        float time = 0f;
        Color c = transitionImage.color;

        while (time < fadeDuration) {

            c.a = Mathf.Lerp(0f, 1f, time / fadeDuration);
            transitionImage.color = c;
            time += Time.deltaTime;
            yield return null;

        }

        c.a = 1f;
        transitionImage.color = c;

    }

    private IEnumerator FadeIn() {

        float time = 0f;
        Color c = transitionImage.color;

        while (time < fadeDuration) {

            c.a = Mathf.Lerp(1f, 0f, time / fadeDuration);
            transitionImage.color = c;
            time += Time.deltaTime;
            yield return null;

        }

        c.a = 0f;
        transitionImage.color = c;
    }
}
