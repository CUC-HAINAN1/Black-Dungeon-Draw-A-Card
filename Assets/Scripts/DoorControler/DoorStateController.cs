using UnityEngine;
using System.Collections;

public class DoorStateController : MonoBehaviour
{
    [Header("尺寸校准")]
    [SerializeField] private Vector2 referenceSize = new Vector2(1.0f, 5.0f); // 标准门尺寸
    [Header("碰撞体配置")]
    [SerializeField] private Collider2D physicsCollider; // 手动绑定非Trigger碰撞体

    [Header("Edgar 门组件绑定")]
    public GameObject upDoor;   // 拖入UpDoor子对象
    public GameObject downDoor; // 拖入DownDoor子对象

    [Header("初始状态")]
    public bool startOpen = true;

    // 新增Edgar方向适配方法
    public void AlignWithEdgar()
    {
        // 适配截图中的Direction=Reset参数
        transform.rotation = Quaternion.identity;

        // 适配Scale=2参数
        //transform.localScale = new Vector3(2, 2, 1);

        // 适配2000坐标系
        transform.position = new Vector2(
            Mathf.Round(transform.position.x * 500f) / 500f,
            Mathf.Round(transform.position.y * 500f) / 500f
        );
    }
    void Start()
    { 
        // 优先处理走廊门
        if (IsCorridorDoor())
        {
            ForceOpenCorridorDoor();
            return;
        }
        // 普通门初始化
        InitializeNormalDoor();
    }

    // 外部调用：关门
    public void CloseDoor(bool instant = false)
    {
        try
        {
            if (!instant)
            {
                // 第一阶段：视觉关闭
                SetDoorState(false);
                StartCoroutine(DelayedColliderEnable());
            }
            else
            {
                // 立即完全关闭
                physicsCollider.isTrigger = false;
                SetDoorState(false);
            }

            UpdateNavigationGraph(downDoor);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"关门失败 {name}: {e.Message}");
        }
    }

    IEnumerator DelayedColliderEnable()
    {
        // 保持碰撞体可穿透0.5秒
        physicsCollider.isTrigger = true;
        yield return new WaitForSeconds(0.5f);

        // 第二阶段：物理阻挡
        physicsCollider.isTrigger = false;
    }
    // 外部调用：开门
    public void OpenDoor()
    {
        try
        {
            physicsCollider.isTrigger = true;
            SetDoorState(true);
            UpdateNavigationGraph(upDoor);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"开门失败 {name}: {e.Message}");
        }
    }

    #region 内部实现
    private bool IsCorridorDoor()
    {
        return transform.parent != null &&
               transform.parent.name.Contains("Corridor");
    }

    private void ForceOpenCorridorDoor()
    {
        startOpen = true;
        SetDoorState(true);
        DisableAllColliders(downDoor);
    }

    private void InitializeNormalDoor()
    {
        SetDoorState(startOpen);
        ToggleColliders(downDoor, enable: !startOpen);
    }

    private void SetDoorState(bool open)
    {
        upDoor.SetActive(open);
        downDoor.SetActive(!open);
    }

    private void ToggleColliders(GameObject target, bool enable)
    {
        if (target == null) return;

        foreach (var collider in target.GetComponentsInChildren<Collider2D>())
        {
            collider.enabled = enable;
            collider.isTrigger = !enable; // 非开门状态时作为物理碰撞
        }
    }

    private void DisableAllColliders(GameObject target)
    {
        if (target == null) return;

        foreach (var collider in target.GetComponentsInChildren<Collider2D>())
        {
            collider.enabled = false;
        }
    }
    public void InitDoorState()
    {
        AlignWithEdgar();
        SetDoorState(startOpen);
    }
    public void ForceInitialize()
    {
        if (upDoor == null || downDoor == null)
        {
            foreach (Transform child in transform)
            {
                if (child.name.Contains("Up")) upDoor = child.gameObject;
                if (child.name.Contains("Down")) downDoor = child.gameObject;
            }
        }

        InitDoorState();
    }
    void Awake()
    {
        // 新增父级缩放重置
        if (transform.parent != null)
        {
            transform.parent.localScale = Vector3.one;
        }
        if (physicsCollider == null)
        {
            physicsCollider = GetComponentInChildren<Collider2D>(includeInactive: true);
            Debug.LogWarning($"手动绑定碰撞体到 {name}", this);
        }
    }
    public void UpdateNavigationGraph(GameObject activePart)
    {
        if (AstarPath.active == null)
        {
            Debug.LogWarning("A* Pathfinding 未初始化");
            return;
        }

        var collider = activePart?.GetComponentInChildren<Collider2D>();
        if (collider != null)
        {
            AstarPath.active.UpdateGraphs(collider.bounds);
        }
        else
        {
            Debug.LogError($"导航更新失败: {activePart?.name} 缺少Collider2D组件");
        }
    }
    #endregion
    // 添加编辑器初始化方法
#if UNITY_EDITOR
    [ContextMenu("Init Doors")]
    private void EditorInitialize()
    {
        Start(); // 允许在编辑器手动初始化
    }
    [ContextMenu("快速修复组件")]
    private void FixComponents()
    {
        var colliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            if (!col.gameObject.CompareTag("Door"))
            {
                col.gameObject.tag = "Door";
            }
        }

        if (gameObject.layer != LayerMask.NameToLayer("Doors"))
        {
            gameObject.layer = LayerMask.NameToLayer("Doors");
        }
    }
#endif
}