using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("血量设置")]
    public int maxHealth;
    public int currentHealth;
    public Slider healthSlider;
    public float flashDuration = 0.1f;
    public Color flashColor = Color.red;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Boss数据")]
    [SerializeField] private BossData bossData;

    void Start()
    {
        maxHealth = bossData.maxHealth;
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        StartCoroutine(FlashEffect());
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator FlashEffect()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }
    }

    void Die() {

        // 死亡动画或效果
        EventManager.Instance.TriggerEvent("BossDied");

        TipManager.Instance.ShowTip("我还会回来的");

        Destroy(gameObject);

    }
}
