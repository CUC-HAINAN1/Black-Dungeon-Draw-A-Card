using UnityEngine;
public class RectRangeIndicator : MonoBehaviour, IRangeIndicator {
    [SerializeField] private Transform rangeRect;
    private CardDataBase.ProjectileParams config;

    private Transform playerTransform;
    private Vector3 _direction;

    
    public void Initialize(CardDataBase cardData) {
        
        playerTransform = PlayerAttributes.Instance.PlayerTransform;

        config = cardData.behaviorConfig.projectile;
        rangeRect.localScale = new Vector3(config.maxRange, 0.1f, 0);

        rangeRect.localPosition = Vector3.zero; 

    }

    public void UpdateIndicator() {
        
        Camera mainCamera = Camera.main;
        Vector3 cameraWorldPos = mainCamera.transform.position;

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(cameraWorldPos.z); // 正交相机：Z值为相机到原点的距离
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        Vector3 playerWorldPos = playerTransform.position;
        _direction = (mouseWorldPos - playerWorldPos).normalized;

        // 抵消父物体X轴翻转的影响
        float parentScaleSign = Mathf.Sign(playerTransform.localScale.x);
        _direction.x *= parentScaleSign;
        _direction.y *= parentScaleSign; 

        // 计算旋转角度（绕Z轴）
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        rangeRect.rotation = Quaternion.Euler(0, 0, angle);
        
    }

    public void Terminate() {
        
        Destroy(gameObject);
    
    }

    public T GetContext<T>() {
        
        if (typeof(T) == typeof(Vector3)) {
        
            return (T)(object)_direction; // 通过装箱转换类型
        
        }

        return default;
    }

}