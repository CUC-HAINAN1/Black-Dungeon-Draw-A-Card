using UnityEngine;

public class CircleRangeIndicator : MonoBehaviour, IRangeIndicator
{
    [SerializeField] private Transform circleTransform;    // 移动加速倍数
    [SerializeField] private float smoothTime = 0.5f;     // 移动平滑时间

    private Vector3 currentVelocity;
    private float maxRadius;
    private Camera mainCamera;
    private Vector3 _mouseWorldPos;

    public void Initialize(CardDataBase cardData) {
    
        mainCamera = Camera.main;
        
        maxRadius = cardData.behaviorConfig.area.radius;
        
        circleTransform.localScale = Vector3.one * maxRadius * 2;

        //脱离父对象主角，圆形位置只受鼠标影响
        circleTransform.SetParent(null);
    
    }

    public void UpdateIndicator() {
        
        // 获取鼠标世界坐标（正交相机处理）
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCamera.transform.position.z);
        _mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        circleTransform.position = Vector3.SmoothDamp(
            circleTransform.position,
            _mouseWorldPos,
            ref currentVelocity,
            smoothTime
        );

    }

    public void Terminate() {
    
        Destroy(gameObject);
    
    }

    public T GetContext<T>() where T : struct {

        if (typeof(T) == typeof(Vector3)) {
        
            return (T)(object)_mouseWorldPos; // 通过装箱转换类型
        
        }

        return default;
    }

}