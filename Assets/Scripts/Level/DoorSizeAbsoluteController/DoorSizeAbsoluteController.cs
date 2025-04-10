// DoorSizeAbsoluteController.cs
using UnityEngine;

public class DoorSizeAbsoluteController : MonoBehaviour
{
    [Header("强制参数")]
    [SerializeField] private Vector2 targetColliderSize = new Vector2(1f, 5f); // 宽1高5
    [SerializeField] private float parentScaleOverride = 1f; // 强制父级缩放

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

        // 强制父级缩放
        parent.localScale = Vector3.one * parentScaleOverride;

        foreach (Transform door in parent)
        {
            // 重置门自身缩放
            door.localScale = Vector3.one;

            // 强制碰撞体尺寸
            var collider = door.GetComponent<BoxCollider2D>();
            if (collider != null)
            {
                collider.size = targetColliderSize;
                collider.offset = Vector2.zero;
            }

        }
    }

#if UNITY_EDITOR
    [ContextMenu("立即执行校准")]
    public void ForceCalibrate()
    {
        Start();
    }
#endif
}
