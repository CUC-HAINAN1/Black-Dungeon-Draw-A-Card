using UnityEngine;
public class SectorRangeIndicator : MonoBehaviour, IRangeIndicator {
    [SerializeField] private Transform rangeSector;
    private CardDataBase.SweepParams config;
    [SerializeField] public Color color;

    private Transform playerTransform;
    private Vector3 _direction;

    public Vector3 Direction => _direction;

    public void Initialize(CardDataBase cardData) {
        
        playerTransform = PlayerAttributes.Instance.PlayerTransform;
        SectorMeshGenerator setcorRangeIndicator = gameObject.GetComponent<SectorMeshGenerator>();

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material.color = color;

        config = cardData.behaviorConfig.sweep;
        setcorRangeIndicator.radius = config.radius;

        rangeSector.localPosition = Vector3.zero; 
    
    }

    public void UpdateIndicator() {
        
        Camera mainCamera = Camera.main;
        Vector3 cameraWorldPos = mainCamera.transform.position;

        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(cameraWorldPos.z); // 正交相机：Z值为相机到原点的距离
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        Vector3 playerWorldPos = playerTransform.position;
        _direction = (mouseWorldPos - playerWorldPos).normalized;

        float parentScaleSign = Mathf.Sign(playerTransform.localScale.x);
        _direction.x *= parentScaleSign;
        _direction.y *= parentScaleSign; 

        // 计算旋转角度（2D场景绕Z轴）
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        rangeSector.rotation = Quaternion.Euler(0, 0, angle);

    }

    public void Terminate() {
        
        Destroy(gameObject);
    
    }

   public T GetContext<T>() where T : struct {

        if (typeof(T) == typeof(Vector3)) {
        
            return (T)(object)_direction; // 通过装箱转换类型
        
        }

        return default;
    }

}