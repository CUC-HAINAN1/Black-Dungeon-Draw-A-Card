using UnityEngine;

public class RoomTriggerController : MonoBehaviour
{
    [Header("关联的门")]
    public DoorStateController[] doors;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var door in doors)
            {
                door.CloseDoor(); // 触发所有门关闭
            }
            // 触发敌人生成逻辑
        }
    }

    // 示例：敌全灭后开门
    public void OnEnemiesDefeated()
    {
        foreach (var door in doors)
        {
            door.OpenDoor();
        }
    }
}
