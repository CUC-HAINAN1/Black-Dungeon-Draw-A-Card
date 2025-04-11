using Edgar.Unity;
using Pathfinding.Examples;
using UnityEngine;
using System.Collections.Generic;
using Google.Events.Protobuf.Cloud.NetworkManagement.V1;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomManager : MonoBehaviour
{ // ����״̬����

    [Header("ս������")]
    [SerializeField] private bool isCombatRoom; // ��Edgarģ��������
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("����ߴ�")]
    [SerializeField] public Vector2 roomSize = new Vector2(11, 11); // �ֶ����óߴ�


    [Header("�ſ���")]
    [SerializeField] public DoorStateController[] doors;
    [Header("����״̬")]
    [SerializeField] private bool isCleared; // ������������״̬���

    private bool hasActiveEnemies;
    private BoxCollider2D roomTrigger;
    private bool isRoomCleared; // ��������״̬���

    [ContextMenu("��ӡ������״̬")]
    void DebugSpawnerStatus()
    {
        if (enemySpawner == null)
            Debug.LogError("EnemySpawnerδ��");
        else
            Debug.Log($"�Ѱ�������������{enemySpawner.spawnPoints.Length}�����ɵ�");
    }
    void AutoFindDoors()
    {
        var doorList = new List<DoorStateController>();
        // ����2��ͨ����ǩ����
        GameObject[] doorObjs = GameObject.FindGameObjectsWithTag("Door");
        foreach (var obj in doorObjs)
        {
            if (obj.transform.IsChildOf(transform)) // ȷ���Ǳ�������Ӷ���
            {
                var controller = obj.GetComponent<DoorStateController>();
                if (controller != null)
                {
                    doorList.Add(controller);
                }
            }
        }

        doors = doorList.ToArray();
    }
    void Awake()
    {
        AutoFindDoors();

        foreach (var door in doors)
        {
            door.InitDoorState(); // ȷ���ŷ�����ȷ
        }
        // ����Edgar��11x11����ߴ�
        roomTrigger = GetComponent<BoxCollider2D>();
        roomTrigger.size = roomSize;
        roomTrigger.isTrigger = true;

        // �Զ�����Edgar���ɵ��ţ���ӦVerDownDoorǰ׺��
        doors = GetComponentsInChildren<DoorStateController>(true);
    }


    public void CloseAllDoors()
    {
        foreach (var door in doors)
        {
            // ����Edgar��Scale=2����
            door.transform.localScale = Vector3.one;
            door.CloseDoor();

        }
    }

    void StartEnemySpawning()
    {
        if (enemySpawner != null)
        {
            // ����Edgar��Scale����
            enemySpawner.transform.localScale = Vector3.one * 2;

            // �������ɵ�����
            foreach (Transform point in enemySpawner.spawnPoints)
            {
                point.position = enemySpawner.GetEdgarAdjustedPosition(point.position);
            }
            enemySpawner.StartSpawning();
            enemySpawner.AllWavesClearedEvent += OnEnemiesCleared;
            // ͬ����״̬
            enemySpawner.SyncWithDoors(doors);
        }
    }

    bool CheckEnemiesInRoom()
    {
        Collider2D[] enemies = Physics2D.OverlapBoxAll(
            (Vector2)transform.position,
            roomTrigger.size,
            0,
            LayerMask.GetMask("Enemy")
        );
        // �����¼��������
        return enemySpawner != null && enemySpawner.activeEnemies.Count > 0;
    }

    void OnEnemiesCleared()
    {
        isCleared = true; // ��Ƿ���Ϊ������
        isRoomCleared = true; // �־û�״̬
        hasActiveEnemies = false;
        OpenAllDoors();
        // ����ȫ��ϵͳ���£������ͼ�е�2000���귶Χ��
        GlobalRoomSystem.UpdateConnectedRooms(transform.position * 1000f);
    }

    void OpenAllDoors()
    {
        foreach (var door in doors)
        {
            door.OpenDoor();
        }
    }

    // Edgar����ת��������2000����ϵ��
    public Vector2 GetGridPosition()
    {
        return new Vector2(
            Mathf.RoundToInt(transform.position.x * 1000),
            Mathf.RoundToInt(transform.position.y * 1000)
        );
    }
    // ��ԭ�������������·���
    private void StartBattle()
    {
        if (!isCombatRoom || isCleared) return;

        // �رձ�����������
        //SceneDoorScanner.Instance.ToggleRoomDoors(this, false);

        // �ر�ȫ�����ţ���������ѡ������һ�֣�
        SceneDoorScanner.Instance.SetAllDoorsOpenState(false);

        StartCoroutine(CombatProcess());
    }

    private IEnumerator CombatProcess()
    {
        enemySpawner.StartSpawning();

        while (enemySpawner.activeEnemies.Count > 0 ||
              !enemySpawner.AllWavesCleared)
        {
            yield return new WaitUntil(() =>
            enemySpawner.AllWavesCleared &&
            enemySpawner.activeEnemies.Count == 0
        );
        }

        EndBattle();
    }

    private void EndBattle()
    {
        isCleared = true;
        //SceneDoorScanner.Instance.ToggleRoomDoors(this, true);

        // ����ȫ�����ţ���������ѡ��
        SceneDoorScanner.Instance.SetAllDoorsOpenState(true);
    }

    // �޸�ԭ��OnTriggerEnter2D
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isCleared) return;

        StartCoroutine(DelayedDoorClose());
    }

    IEnumerator DelayedDoorClose()
    {
        // ���ֿ���״̬�ȴ���ҽ���
        yield return new WaitForSeconds(0.2f); // 0.2�뻺��ʱ��

        CloseAllDoors();
        StartBattle();
    }
    void OnDestroy()
    {
        if (enemySpawner != null)
        {
            enemySpawner.AllWavesClearedEvent -= OnEnemiesCleared;
        }
    }
}
