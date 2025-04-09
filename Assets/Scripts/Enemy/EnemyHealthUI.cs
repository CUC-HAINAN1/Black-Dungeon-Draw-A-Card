// EnemyHealthUI.cs ��Ҫ��������ʵ��
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("UI���")]
    [SerializeField] private Slider healthSlider; // �ֶ��󶨽�ͼ2�е�Slider���
    [SerializeField] private Image fillImage;     // �ֶ��󶨽�ͼ4�е�Fillͼ��

    [Header("��ɫ����")]
    [SerializeField] private Gradient colorGradient;

    public void Initialize(int maxHealth)
    {
        // ǿ���������ţ��޸���ͼ1���⣩
        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        canvasRect.localScale = Vector3.one;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        fillImage.color = colorGradient.Evaluate(1f);
    }

    public void UpdateHealth(int currentHealth)
    {
        // ȷ����UI�̸߳���
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