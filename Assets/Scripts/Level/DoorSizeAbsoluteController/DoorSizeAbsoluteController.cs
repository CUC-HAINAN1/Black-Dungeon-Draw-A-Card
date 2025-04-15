// DoorSizeAbsoluteController.cs
using UnityEngine;

public class DoorSizeAbsoluteController : MonoBehaviour
{
    [Header("ǿ�Ʋ���")]
    [SerializeField] private Vector2 targetColliderSize = new Vector2(1f, 5f); // ��1��5
    [SerializeField] private float parentScaleOverride = 1f; // ǿ�Ƹ�������

    void Start()
    {
        ProcessDoors("HorDownDoorDOWN");
        ProcessDoors("HorDownDoorUP");
        ProcessDoors("VerDownDoorLEFT");
        ProcessDoors("VerDownDoor RIGHT");
    }

    void ProcessDoors(string parentName)
    {
        Transform parent = GameObject.Find(parentName)?.transform;
        if (parent == null) return;

        // ǿ�Ƹ�������
        parent.localScale = Vector3.one * parentScaleOverride;

        foreach (Transform door in parent)
        {
            // ��������������
            door.localScale = Vector3.one;

            // ǿ����ײ��ߴ�
            var collider = door.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.size = targetColliderSize;
                collider.offset = Vector2.zero;
            }
    
        }
    }

#if UNITY_EDITOR
    [ContextMenu("����ִ��У׼")]
    public void ForceCalibrate()
    {
        Start();
    }
#endif
}
