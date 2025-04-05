using UnityEngine;
using UnityEngine.UI;

public class StatusBarsUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider shieldSlider;

    //平滑移动
    [SerializeField] private float smoothTime = 0.3f;
    private float healthVelocity;
    private float shieldVelocity;
    private float targetHealth;
    private float targetShield;


    private void Start() {
        
        // 初始化目标值
        targetHealth = PlayerAttributes.Instance.Health;
        targetShield = PlayerAttributes.Instance.Shield;

        // 初始化血条
        healthSlider.maxValue = PlayerAttributes.Instance._MaxHealth;
        healthSlider.value = targetHealth;
        
        // 初始化盾条
        shieldSlider.maxValue = PlayerAttributes.Instance._MaxShield;
        shieldSlider.value = targetShield;

        // 订阅事件
        EventManager.Instance.Subscribe("HealthChanged", OnHealthChanged);
        EventManager.Instance.Subscribe("ShieldChanged", OnShieldChanged);
    
    }

    private void Update() {

    // 平滑更新血条
        healthSlider.value = Mathf.SmoothDamp(
        
        healthSlider.value, 
        targetHealth, 
        ref healthVelocity, 
        smoothTime
    
    );

    // 平滑更新盾条
        shieldSlider.value = Mathf.SmoothDamp(
        
        shieldSlider.value,
        targetShield,
        ref shieldVelocity,
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

    private void OnDestroy() {
    
        if (EventManager.Instance == null) return;
        
        EventManager.Instance.Unsubscribe("HealthChanged", OnHealthChanged);
        EventManager.Instance.Unsubscribe("ShieldChanged", OnShieldChanged);
    
    }

}