using UnityEngine;

public class DoorStateController : MonoBehaviour
{
    [Header("Edgar 门组件绑定")]
    public GameObject upDoor;   // 拖入UpDoor子对象
    public GameObject downDoor; // 拖入DownDoor子对象

    [Header("初始状态")]
    public bool startOpen = true;

    private Collider2D doorCollider;

    public void Start()
    {
        // 初始化状态
        upDoor.SetActive(startOpen);
        downDoor.SetActive(!startOpen);

        // 获取门的碰撞器（假设DownDoor是关闭状态时的碰撞器）
        doorCollider = downDoor.GetComponentInChildren<Collider2D>();
        if (doorCollider != null)
            doorCollider.enabled = !startOpen;
    }

    // 外部调用：关门
    public void CloseDoor()
    {
        upDoor.SetActive(false);
        downDoor.SetActive(true);
        EnableDownDoorsColliders();
    }
    private void EnableDownDoorsColliders()
    {
        Collider2D[] colliders = downDoor.GetComponentsInChildren<Collider2D>();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
            collider.isTrigger = false; // 确保不是Trigger
        }
    }
    // 外部调用：开门
    public void OpenDoor()
    {
        upDoor.SetActive(true);
        downDoor.SetActive(false);
        if (doorCollider != null)
            doorCollider.enabled = false;
    }
}