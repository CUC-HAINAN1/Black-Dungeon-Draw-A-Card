using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingPageController : MonoBehaviour
{
    [Header("UI 组件")]
    public Slider slider;
    public GameObject interactionPanel;

    public GameObject prompt1;
    public GameObject prompt2;

    public Button button1;
    public Button button2;

    public float threshold = 10f; // 累计滑动的阈值

    private float cumulativeDelta = 0f;
    private float lastSliderValue;

    void Start() {
        
        if (slider != null) {
            lastSliderValue = slider.value;
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        if (button1 != null)
            button1.onClick.AddListener(OnButton1Clicked);

        if (button2 != null)
            button2.onClick.AddListener(OnButton2Clicked);

        if (interactionPanel != null)
            interactionPanel.SetActive(false);

    }

    void OnSliderValueChanged(float newValue) {

        float delta = Mathf.Abs(newValue - lastSliderValue);
        cumulativeDelta += delta;
        lastSliderValue = newValue;

        if (cumulativeDelta >= threshold && !interactionPanel.activeSelf) {

            interactionPanel.SetActive(true);

            if (prompt1 != null)
                prompt1.SetActive(true);

            if (prompt2 != null)
                prompt2.SetActive(false);


        }

    }

    void OnButton1Clicked() {

        if (prompt1 != null)
            prompt1.SetActive(false);

        if (prompt2 != null)
            prompt2.SetActive(true);

    }

    void OnButton2Clicked() {

        gameObject.SetActive(false);

    }
}
