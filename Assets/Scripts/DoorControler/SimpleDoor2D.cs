using UnityEngine;

public class SimpleDoor2D : MonoBehaviour
{
    private Collider2D doorCollider;

    void Start()
    {
        doorCollider = GetComponent<Collider2D>();
        doorCollider.enabled = false; // ��ʼΪ����״̬
    }

    public void CloseDoor()
    {
        doorCollider.enabled = true;
        Debug.Log("���ѹر�");
    }

    public void OpenDoor()
    {
        doorCollider.enabled = false;
        Debug.Log("���ѿ���");
    }
}