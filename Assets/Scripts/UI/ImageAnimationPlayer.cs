using UnityEngine;
using UnityEngine.UI;

public class ImageAnimationPlayer : MonoBehaviour {
    public Sprite[] frames;
    public float frameRate = 10f;

    private Image image;
    private int currentFrame;
    private float timer;

    void Start() {

        image = GetComponent<Image>();

    }

    void Update() {

        if (frames.Length == 0) return;

        timer += Time.deltaTime;

        if (timer >= 1f / frameRate) {
            currentFrame = (currentFrame + 1) % frames.Length;
            image.sprite = frames[currentFrame];
            timer = 0f;

        }
    }
}
