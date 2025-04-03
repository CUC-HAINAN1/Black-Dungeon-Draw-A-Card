using UnityEngine;
public interface IRangeIndicator {
    void Initialize(CardDataBase cardData);  // 初始化方法
    void UpdateIndicator(Vector2 screenDelta); // 拖拽时更新
    void Terminate(); // 结束指示
}

//火球术的旋转矩形
public class FireballRangeIndicator : MonoBehaviour, IRangeIndicator {
    [SerializeField] private Transform rangeRect;
    [SerializeField] private float rotationSpeed = 90f;
    private CardDataBase.ProjectileParams config;

    public void Initialize(CardDataBase cardData) {
        
        config = cardData.behaviorConfig.projectile;
        rangeRect.localScale = new Vector3(1, 1, config.maxRange);
    
    }

    public void UpdateIndicator(Vector2 delta) {
        // 根据拖拽方向旋转
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        rangeRect.rotation = Quaternion.Euler(0, angle, 0);
        
        // 持续旋转效果
        rangeRect.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    
    }

    public void Terminate() {
        
        Destroy(gameObject);
    
    }
}