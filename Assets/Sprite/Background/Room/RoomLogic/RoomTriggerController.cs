using UnityEngine;

public class RoomTriggerController : MonoBehaviour
{
    [Header("��������")]
    public DoorStateController[] doors;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var door in doors)
            {
                door.CloseDoor(); // ���������Źر�
            }
            // �������������߼�
        }
    }

    // ʾ������ȫ�����
    public void OnEnemiesDefeated()
    {
        foreach (var door in doors)
        {
            door.OpenDoor();
        }
    }
}
