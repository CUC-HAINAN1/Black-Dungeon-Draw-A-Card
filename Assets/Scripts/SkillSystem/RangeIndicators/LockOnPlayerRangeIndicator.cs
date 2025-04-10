using UnityEngine;

public class LockOnPlayerRangeIndicator : MonoBehaviour, IRangeIndicator {

    [SerializeField] private Transform ellipseTransform;

    private Transform playerTransform;
    private Vector3 _targetOffset = new Vector3(0, 0.3f, 0); // Y 轴向上偏移
    private Vector3 _direction;

    public void Initialize(CardDataBase cardData) {
        playerTransform = PlayerAttributes.Instance.PlayerTransform;

        // 设置椭圆大小
        ellipseTransform.localScale = new Vector3(0.5f, 0.7f, 1f);
        ellipseTransform.localPosition = Vector3.zero;
        Debug.Log("Player Locked");

    }

    public void UpdateIndicator() {
        
        if (playerTransform == null) return;

        // 直接跟随玩家位置偏移
        transform.position = playerTransform.position + _targetOffset;
    }

    public void Terminate() {
        
        Destroy(gameObject);
    
    }

    public T GetContext<T>() {
        if (typeof(T) == typeof(Vector3)) {
            return (T)(object)_direction;
        }

        return default;
    }

}