using UnityEngine;

public class DoorStateController : MonoBehaviour
{
    [Header("Edgar �������")]
    public GameObject upDoor;   // ����UpDoor�Ӷ���
    public GameObject downDoor; // ����DownDoor�Ӷ���

    [Header("��ʼ״̬")]
    public bool startOpen = true;

    private Collider2D doorCollider;

    public void Start()
    {
        // ��ʼ��״̬
        upDoor.SetActive(startOpen);
        downDoor.SetActive(!startOpen);

        // ��ȡ�ŵ���ײ��������DownDoor�ǹر�״̬ʱ����ײ����
        doorCollider = downDoor.GetComponentInChildren<Collider2D>();
        if (doorCollider != null)
            doorCollider.enabled = !startOpen;
    }

    // �ⲿ���ã�����
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
            collider.isTrigger = false; // ȷ������Trigger
        }
    }
    // �ⲿ���ã�����
    public void OpenDoor()
    {
        upDoor.SetActive(true);
        downDoor.SetActive(false);
        if (doorCollider != null)
            doorCollider.enabled = false;
    }
}