// EnemyHealthUI.cs 需要包含以下实现
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Slider healthSlider; // 手动绑定截图2中的Slider组件
    [SerializeField] private Image fillImage;     // 手动绑定截图4中的Fill图像

    [Header("颜色渐变")]
    [SerializeField] private Gradient colorGradient;

    public void Initialize(int maxHealth)
    {
        // 强制重置缩放（修复截图1问题）
        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        canvasRect.localScale = Vector3.one;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        fillImage.color = colorGradient.Evaluate(1f);
    }

    public void UpdateHealth(int currentHealth)
    {
        // 确保在UI线程更新
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
            fillImage.color = colorGradient.Evaluate(healthSlider.normalizedValue);
        }
    }

    public void Hide()
    {
        healthSlider.gameObject.SetActive(false);
    }
}