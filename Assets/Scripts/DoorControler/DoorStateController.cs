using UnityEngine;
using System.Collections;

public class DoorStateController : MonoBehaviour {
    [Header("�ߴ�У׼")]
    [SerializeField] private Vector2 referenceSize = new Vector2(1.0f, 5.0f); // ��׼�ųߴ�
    [Header("��ײ������")]
    [SerializeField] private Collider2D physicsCollider; // �ֶ��󶨷�Trigger��ײ��

    [Header("Edgar �������")]
    public GameObject upDoor;   // ����UpDoor�Ӷ���
    public GameObject downDoor; // ����DownDoor�Ӷ���

    public GameObject FloorTrigger1;
    public GameObject FloorTrigger2;

    [Header("��ʼ״̬")]
    public bool startOpen = true;

    // ����Edgar�������䷽��
    public void AlignWithEdgar() {
        // �����ͼ�е�Direction=Reset����
        transform.rotation = Quaternion.identity;

        // ����Scale=2����
        //transform.localScale = new Vector3(2, 2, 1);

        // ����2000����ϵ
        transform.position = new Vector2(
            Mathf.Round(transform.position.x * 500f) / 500f,
            Mathf.Round(transform.position.y * 500f) / 500f
        );
    }
    void Start() {
        // ���ȴ���������
        if (IsCorridorDoor()) {
            ForceOpenCorridorDoor();
            return;
        }
        // ��ͨ�ų�ʼ��
        StartCoroutine(InitializeNormalDoor());
    }

    // �ⲿ���ã�����
    public void CloseDoor(bool instant = false) {
        try {
            if (!instant) {
                // ��һ�׶Σ��Ӿ��ر�
                SetDoorState(false);
                StartCoroutine(DelayedColliderEnable());
            }
            else {
                // ������ȫ�ر�
                physicsCollider.isTrigger = false;
                SetDoorState(false);
            }

            UpdateNavigationGraph(downDoor);
        }
        catch (System.Exception e) {
            CustomLogger.LogError($"����ʧ�� {name}: {e.Message}");
        }
    }

    IEnumerator DelayedColliderEnable() {
        // ������ײ��ɴ�͸0.5��
        physicsCollider.isTrigger = true;
        yield return new WaitForSeconds(0.5f);

        // �ڶ��׶Σ������赲
        physicsCollider.isTrigger = false;
    }
    // �ⲿ���ã�����
    public void OpenDoor() {

        if (!FloorTrigger1.GetComponent<FloorDetector>().IsFloorExisting ||
            !FloorTrigger2.GetComponent<FloorDetector>().IsFloorExisting)
            return;

        try {
                physicsCollider.isTrigger = true;
                SetDoorState(true);
                UpdateNavigationGraph(upDoor);
            }
            catch (System.Exception e) {
                CustomLogger.LogError($"����ʧ�� {name}: {e.Message}");
            }

    }

    #region �ڲ�ʵ��
    private bool IsCorridorDoor() {
        return transform.parent != null &&
               transform.parent.name.Contains("Corridor");
    }

    private void ForceOpenCorridorDoor() {
        startOpen = true;
        SetDoorState(true);
        //DisableAllColliders(downDoor);
    }

    private IEnumerator InitializeNormalDoor() {

            yield return new WaitForSeconds(1f);

        if (!FloorTrigger1.GetComponent<FloorDetector>().IsFloorExisting ||
            !FloorTrigger2.GetComponent<FloorDetector>().IsFloorExisting) {

            CustomLogger.LogWarning("No floors, keep the door close");

            SetDoorState(false);
            ToggleColliders(downDoor, enable: true);

        }
        else {

            SetDoorState(startOpen);
            //ToggleColliders(downDoor, enable: !startOpen);

        }
    }

    private void SetDoorState(bool open) {

        Debug.LogWarning($"[{name}] SetDoorState called. Open = {open}");

        upDoor.SetActive(open);
        downDoor.SetActive(!open);
    }

    private void ToggleColliders(GameObject target, bool enable) {
        if (target == null)
            return;

        foreach (var collider in target.GetComponentsInChildren<Collider2D>()) {
            collider.enabled = enable;
            collider.isTrigger = !enable; // �ǿ���״̬ʱ��Ϊ������ײ
        }
    }

    private void DisableAllColliders(GameObject target) {
        if (target == null)
            return;

        foreach (var collider in target.GetComponentsInChildren<Collider2D>()) {
            collider.enabled = false;
        }
    }
    public void InitDoorState() {

        AlignWithEdgar();
        InitializeNormalDoor();

    }
    public void ForceInitialize() {
        if (upDoor == null || downDoor == null) {
            foreach (Transform child in transform) {
                if (child.name.Contains("Up"))
                    upDoor = child.gameObject;
                if (child.name.Contains("Down"))
                    downDoor = child.gameObject;
            }
        }

        InitDoorState();
    }
    void Awake() {

        // ����������������
        if (transform.parent != null) {
            transform.parent.localScale = Vector3.one;
        }
        if (physicsCollider == null) {
            physicsCollider = GetComponentInChildren<Collider2D>(includeInactive: true);
            CustomLogger.LogWarning($"�ֶ�����ײ�嵽 {name}");
        }
    }
    public void UpdateNavigationGraph(GameObject activePart) {
        if (AstarPath.active == null) {
            CustomLogger.LogWarning("A* Pathfinding δ��ʼ��");
            return;
        }

        var collider = activePart?.GetComponentInChildren<Collider2D>();
        if (collider != null) {
            AstarPath.active.UpdateGraphs(collider.bounds);
        }
        else {
            //CustomLogger.LogError($"��������ʧ��: {activePart?.name} ȱ��Collider2D���");
        }
    }
    #endregion
    // ���ӱ༭����ʼ������
#if UNITY_EDITOR
    [ContextMenu("Init Doors")]
    private void EditorInitialize() {
        Start(); // �����ڱ༭���ֶ���ʼ��
    }
    [ContextMenu("�����޸����")]
    private void FixComponents() {
        var colliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders) {
            if (!col.gameObject.CompareTag("Door")) {
                col.gameObject.tag = "Door";
            }
        }

        if (gameObject.layer != LayerMask.NameToLayer("Doors")) {
            gameObject.layer = LayerMask.NameToLayer("Doors");
        }
    }
#endif
}
