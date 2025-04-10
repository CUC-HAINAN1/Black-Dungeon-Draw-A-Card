using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Analytics;

public class LockOnEnemyRangeIndicator : MonoBehaviour, IRangeIndicator {

    [SerializeField] private Transform ellipseTransform;
    [SerializeField] private float lockRange = 1f; //锁定范围
    [SerializeField] private float smoothTime = 0.03f;

    private List<GameObject> targetsInRange = new(); //当前鼠标指向范围内的所有敌人
    private int currentTargetIndex = 0;

    private GameObject currentTarget; //当前锁定的敌人
    private Vector3 lastMousePosition;
    private Vector3 _mouseWorldPos;
    private Camera mainCamera;
    private Vector3 currentVelocity;
    private float _accumulatedDeltaX; //鼠标移动累计增量

    private Vector3 _targetOffset = new Vector3(0, 0.3f, 0);

    public void Initialize(CardDataBase cardData) {
        
        ellipseTransform.localScale = new Vector3(0.5f, 0.7f, 1f); 
        mainCamera = Camera.main;

        UpdateTargetList();
        lastMousePosition = Input.mousePosition;
        ellipseTransform.SetParent(null);
        ellipseTransform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
    }

    public void UpdateIndicator() {
        
        UpdateTargetList();
        UpdateLockOnSelection();
        UpdateEllipsePosition();
    
    }

    private void UpdateTargetList() {
        
        targetsInRange.Clear();
        
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCamera.transform.position.z); // 确保 Z 轴与相机一致
        _mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        
        //查找所有敌人
        Collider2D[] Enemys = Physics2D.OverlapCircleAll(_mouseWorldPos, lockRange);
        
        foreach (var EnemyCol in Enemys) {

            if (EnemyCol.CompareTag("Enemy")) {
                
                Debug.Log(EnemyCol.name);
                targetsInRange.Add(EnemyCol.gameObject);
            
            }
        }

        // 先按屏幕 X 坐标排序，让左右滑动符合空间感
        targetsInRange = targetsInRange
            .OrderBy(t => Camera.main.WorldToScreenPoint(t.transform.position).x)
            .ToList();

        // 如果当前目标不在范围内，或已经被销毁，就自动切换目标
        if (targetsInRange.Count > 0) {
            if (currentTarget == null || !targetsInRange.Contains(currentTarget)) {
                
                currentTargetIndex = 0;
                currentTarget = targetsInRange[currentTargetIndex];
            
            } else {
                // 更新当前索引,保持同步
                currentTargetIndex = targetsInRange.IndexOf(currentTarget);
            
            }
        
        } else {
            
            currentTarget = null;
        
        }
        
    }
    private void UpdateLockOnSelection() {
        
        //没有目标或者只找到一个就不用切换
        if (targetsInRange.Count <= 1) return;

        Vector3 currentMousePos = Input.mousePosition;
        float deltaX = currentMousePos.x - lastMousePosition.x;

        // 累积增量，同方向持续累加，反方向则重置
        if (Mathf.Sign(deltaX) == Mathf.Sign(_accumulatedDeltaX) || _accumulatedDeltaX == 0) {
            
            _accumulatedDeltaX += deltaX;
        
        } else {
            
            _accumulatedDeltaX = deltaX; // 方向变化时重置累积
        
        }

        // 当累积量超过阈值时切换
        if (Mathf.Abs(_accumulatedDeltaX) > 100f) {
            
            if (_accumulatedDeltaX > 0) {
                
                currentTargetIndex = (currentTargetIndex + 1) % targetsInRange.Count;
            
            } else {
                
                currentTargetIndex = (currentTargetIndex - 1 + targetsInRange.Count) % targetsInRange.Count;
            
            }
            
            currentTarget = targetsInRange[currentTargetIndex];
            _accumulatedDeltaX = 0; // 重置累积量
        
        }

        lastMousePosition = currentMousePos; // 始终更新上一帧鼠标位置
        
        }

    private void UpdateEllipsePosition() {
        
        Vector3 targetPosition;
    
        // 统一计算目标位置
        if (currentTarget != null) {
            
            targetPosition = currentTarget.transform.position + _targetOffset;
        
        } else {
            
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = Mathf.Abs(mainCamera.transform.position.z);
            targetPosition = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        
        }

        // 始终使用 SmoothDamp 移动
        ellipseTransform.position = Vector3.SmoothDamp(
            ellipseTransform.position,
            targetPosition,
            ref currentVelocity, 
            smoothTime
        );
    
    }

    public void Terminate() {
        
        Destroy(gameObject);
    
    }

    public T GetContext<T>() {
        
        if (typeof(T) == typeof(GameObject)) {
            
            return (T)(object)currentTarget;
        
        }

        return default;
    }
}