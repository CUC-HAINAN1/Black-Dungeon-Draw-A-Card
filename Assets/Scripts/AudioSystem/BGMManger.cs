using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour {
    public static BGMManager Instance { get; private set; }

    public AudioSource bgmSource;
    public AudioClip normalBGM;
    public AudioClip battleBGM;
    public AudioClip menuBGM;
    public AudioClip BossBGM;

    private void Awake() {

        if (Instance != null && Instance != this) {

            Destroy(gameObject);
            return;

        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    public void PlayBGM(AudioClip clip, float fadeDuration = 1f) {

        if (bgmSource.clip == clip)
            return;

        StartCoroutine(SwitchBGM(clip, fadeDuration));

    }

    private IEnumerator SwitchBGM(AudioClip newClip, float duration) {
        // 淡出
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime) {
            bgmSource.volume = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.loop = true;
        bgmSource.Play();

        // 淡入
        for (float t = 0; t < duration; t += Time.unscaledDeltaTime) {
            bgmSource.volume = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }
    }
}
