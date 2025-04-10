using UnityEngine;

public class RoomTriggerController : MonoBehaviour
{
    [Header("房间设置")]
    [SerializeField] private DoorStateController[] doors;
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("调试")]
    [SerializeField] private Color gizmoColor = new Color(1, 0, 0, 0.2f);
    [SerializeField] private bool alwaysShowGizmos = true;

    private bool isCleared;
    private bool isActive;

    void OnValidate()
    {
        // 编辑器模式自动获取引用
#if UNITY_EDITOR
        if (doors == null || doors.Length == 0)
        {
            var doorList = new System.Collections.Generic.List<DoorStateController>();
            foreach (Transform child in transform.parent)
            {
                if (child.name.Contains("VerDownDoor"))
                {
                    var door = child.GetComponent<DoorStateController>();
                    if (door != null) doorList.Add(door);
                }
            }
            doors = doorList.ToArray();
        }

        if (enemySpawner == null)
            enemySpawner = GetComponentInChildren<EnemySpawner>();
#endif
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCleared || !other.CompareTag("Player")) return;
        if (enemySpawner == null)
        {
            ToggleDoors(true);
            return;
        }
        // 安全校验
        if (!ValidateComponents())
            return;

        isActive = true;
        ToggleDoors(false);
        if (enemySpawner != null)
        {
            enemySpawner.StartSpawning();
            enemySpawner.AllWavesClearedEvent += OnRoomCleared;
        }
    }

    private bool ValidateComponents()
    {
        bool isValid = true;

        if (doors == null || doors.Length == 0)
        {
            Debug.LogError($"房间 {name} 未配置门控制器!", this);
            isValid = false;
        }
        else
        {
            foreach (var door in doors)
            {
                if (door == null)
                {
                    Debug.LogError($"房间 {name} 存在未配置的门控制器!", this);
                    isValid = false;
                }
            }
        }

        if (enemySpawner == null)
        {
            Debug.LogError($"房间 {name} 未配置敌人生成器!", this);
            isValid = false;
        }

        return isValid;
    }

    private void OnRoomCleared()
    {
        isCleared = true;
        ToggleDoors(true);
    }

    private void ToggleDoors(bool open)
    {
        foreach (var door in doors)
        {
            if (door == null) continue;

            try
            {
                if (open) door.OpenDoor();
                else door.CloseDoor();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"门操作失败: {door.name}\n{e.Message}", this);
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!alwaysShowGizmos) return;

        var collider = GetComponent<Collider2D>();
        if (collider == null) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
    }
#endif

    // 测试方法
    [ContextMenu("测试房间逻辑")]
    public void DebugTestRoom()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("仅在运行模式可用");
            return;
        }

        Debug.Log("=== 开始房间测试 ===");
        OnTriggerEnter2D(GetComponent<Collider2D>());
        DebugKillAll();
    }

    public void DebugKillAll()
    {
        if (enemySpawner != null)
            enemySpawner.KillAllEnemies();
    }

}
