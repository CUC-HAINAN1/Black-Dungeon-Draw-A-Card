using UnityEngine;

public class RoomTriggerController : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private DoorStateController[] doors;
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("����")]
    [SerializeField] private Color gizmoColor = new Color(1, 0, 0, 0.2f);
    [SerializeField] private bool alwaysShowGizmos = true;

    private bool isCleared;
    private bool isActive;

    void OnValidate()
    {
        // �༭��ģʽ�Զ���ȡ����
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
        // ��ȫУ��
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
            Debug.LogError($"���� {name} δ�����ſ�����!", this);
            isValid = false;
        }
        else
        {
            foreach (var door in doors)
            {
                if (door == null)
                {
                    Debug.LogError($"���� {name} ����δ���õ��ſ�����!", this);
                    isValid = false;
                }
            }
        }

        if (enemySpawner == null)
        {
            Debug.LogError($"���� {name} δ���õ���������!", this);
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
                Debug.LogError($"�Ų���ʧ��: {door.name}\n{e.Message}", this);
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

    // ���Է���
    [ContextMenu("���Է����߼�")]
    public void DebugTestRoom()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("��������ģʽ����");
            return;
        }

        Debug.Log("=== ��ʼ������� ===");
        OnTriggerEnter2D(GetComponent<Collider2D>());
        DebugKillAll();
    }

    public void DebugKillAll()
    {
        if (enemySpawner != null)
            enemySpawner.KillAllEnemies();
    }

}
