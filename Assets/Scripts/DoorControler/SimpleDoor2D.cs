using UnityEngine;

public class SimpleDoor2D : MonoBehaviour
{
    private Collider2D doorCollider;

    void Start()
    {
        doorCollider = GetComponent<Collider2D>();
        doorCollider.enabled = false; // 初始为开启状态
    }

    public void CloseDoor()
    {
        doorCollider.enabled = true;
        Debug.Log("门已关闭");
    }

    public void OpenDoor()
    {
        doorCollider.enabled = false;
        Debug.Log("门已开启");
    }
}