using UnityEngine;
using UnityEngine.UI;

public class StatusBarsUI : MonoBehaviour {
    [Header("绑定的 Slider")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider shieldSlider;
    [SerializeField] private Slider manaSlider;

    [Header("平滑移动参数")]
    [SerializeField] private float smoothTime = 0.3f;

    private float healthVelocity;
    private float shieldVelocity;
    private float manaVelocity;

    private float targetHealth;
    private float targetShield;
    private float targetMana;

    private void Start() {
    
       
        var playerAttributes = PlayerAttributes.Instance;

        targetHealth = playerAttributes.Health;
        targetShield = playerAttributes.Shield;
        targetMana = playerAttributes.Mana;

        // 初始化各 Slider 最大值
        healthSlider.maxValue = playerAttributes._MaxHealth;
        shieldSlider.maxValue = playerAttributes._MaxShield;
        manaSlider.maxValue = playerAttributes._MaxMana;

        // 初始化当前值
        healthSlider.value = targetHealth;
        shieldSlider.value = targetShield;
        manaSlider.value = targetMana;

        EventManager.Instance.Subscribe("HealthChanged", OnHealthChanged);
        EventManager.Instance.Subscribe("ShieldChanged", OnShieldChanged);
        EventManager.Instance.Subscribe("ManaChanged", OnManaChanged);
    
    }

    private void Update() {
        
        
        healthSlider.value = Mathf.SmoothDamp(
            healthSlider.value,
            targetHealth,
            ref healthVelocity,
            smoothTime
        );

        
        shieldSlider.value = Mathf.SmoothDamp(
            shieldSlider.value,
            targetShield,
            ref shieldVelocity,
            smoothTime
        );

      
        manaSlider.value = Mathf.SmoothDamp(
            manaSlider.value,
            targetMana,
            ref manaVelocity,
            smoothTime
        );
    
    }

    private void OnHealthChanged(object data) {
        
        var changeData = (PlayerAttributes.AttributeChangeData)data;
        healthSlider.maxValue = PlayerAttributes.Instance._MaxHealth;
        targetHealth = changeData.CurrentValue;

        healthSlider.gameObject.SetActive(changeData.CurrentValue >= 0);
    
    }

    private void OnShieldChanged(object data) {
        
        var changeData = (PlayerAttributes.AttributeChangeData)data;
        shieldSlider.maxValue = PlayerAttributes.Instance._MaxShield;
        targetShield = changeData.CurrentValue;

        shieldSlider.gameObject.SetActive(changeData.CurrentValue >= 0);
    
    }

    private void OnManaChanged(object data) {
        
        var changeData = (PlayerAttributes.AttributeChangeData)data;
        manaSlider.maxValue = PlayerAttributes.Instance._MaxMana;
        targetMana = changeData.CurrentValue;

        manaSlider.gameObject.SetActive(changeData.CurrentValue >= 0);
    
    }

    private void OnDestroy() {
        
        if (EventManager.Instance == null) return;

        EventManager.Instance.Unsubscribe("HealthChanged", OnHealthChanged);
        EventManager.Instance.Unsubscribe("ShieldChanged", OnShieldChanged);
        EventManager.Instance.Unsubscribe("ManaChanged", OnManaChanged);
    
    }

}