using Edgar.Unity;
using Pathfinding.Examples;
using UnityEngine;
using System.Collections.Generic;
using Google.Events.Protobuf.Cloud.NetworkManagement.V1;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomManager : MonoBehaviour
{
    [Header("宝箱生成设置")]
    [SerializeField] private GameObject chestPrefab;  // 宝箱预制体
    [SerializeField] private Transform[] chestSpawnPoints; // 生成点数组
    private bool chestSpawned = false; // 防止重复生成

    public event System.Action OnRoomCleared; // 添加事件声明
    // ����״̬����
    // 改用HashSet生成房间
    private static HashSet<Vector3Int> spawnedRooms = new HashSet<Vector3Int>();
    private Vector3Int roomCoordinate;

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
            CustomLogger.LogError("EnemySpawnerδ��");
        else
            CustomLogger.Log($"�Ѱ�������������{enemySpawner.spawnPoints.Length}�����ɵ�");
    }


    void SpawnChest()
    {

        CustomLogger.Log($"宝箱生成中");

        // 防止重复生成
        if (chestSpawned || chestPrefab == null || chestSpawnPoints.Length == 0)
            return;

        // 随机选择生成点
        int randomIndex = Random.Range(0, chestSpawnPoints.Length);
        Transform spawnPoint = chestSpawnPoints[randomIndex];

        // 物理检测防止卡墙
        //Collider2D hit = Physics2D.OverlapCircle(spawnPoint.position, 0.5f);
            Instantiate(chestPrefab, spawnPoint.position, Quaternion.identity);
            chestSpawned = true;
            CustomLogger.Log($"宝箱生成成功！位置：{spawnPoint.position}");

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
        //roomTrigger = GetComponent<BoxCollider2D>();
        //roomTrigger.size = roomSize;
        //roomTrigger.isTrigger = true;

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
        if (enemySpawner == null) return;

        // 先取消旧订阅
        enemySpawner.AllWavesClearedEvent -= OnEnemiesCleared;
        // 再添加新订阅
        enemySpawner.AllWavesClearedEvent += OnEnemiesCleared;

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
        // 触发宝箱显示事件
        CustomLogger.LogWarning("即将触发OnRoomCleared事件");
        OnRoomCleared?.Invoke(); // 新增触发代码
        // 添加调试日志
        // ����ȫ��ϵͳ���£������ͼ�е�2000���귶Χ��
        GlobalRoomSystem.UpdateConnectedRooms(transform.position * 1000f);

        CustomLogger.LogWarning("即将触发宝箱生成");

        SpawnChest(); // 添加生成调用
    }
    // 新增宝箱生成方法

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
    private void StartBattle() {
        if (!isCombatRoom || isCleared)
            return;

        // �رձ�����������
        //SceneDoorScanner.Instance.ToggleRoomDoors(this, false);

        // �ر�ȫ�����ţ���������ѡ������һ�֣�
        SceneDoorScanner.Instance.SetAllDoorsOpenState(false);

        StartCoroutine(CombatProcess());

        if (GameObject.FindGameObjectsWithTag("Boss").Length == 0) {

            BGMManager.Instance.PlayBGM(BGMManager.Instance.battleBGM);

        }

        else {

            if (BossEventManager.Instance == null)
                CustomLogger.LogError("BossEventManager not found");

            BossEventManager.Instance.PlayBossCinematic(GameObject.FindGameObjectsWithTag("Boss")[0].transform);

        }
    }

    private IEnumerator CombatProcess() {
        enemySpawner.StartSpawning();

        // 先取消旧订阅
        enemySpawner.AllWavesClearedEvent -= OnEnemiesCleared;
        // 再添加新订阅
        enemySpawner.AllWavesClearedEvent += OnEnemiesCleared;

        while (enemySpawner.activeEnemies.Count > 0 ||
              !enemySpawner.AllWavesCleared) {
            yield return new WaitUntil(() =>
            enemySpawner.AllWavesCleared &&
            enemySpawner.currentWaveIndex != -1 &&
            enemySpawner.waves[enemySpawner.currentWaveIndex].enemyCount == enemySpawner.currentWaveDeadEnemyCount

        );
        }

        EndBattle();
        PlayerAttributes.Instance.Heal(100);
    }

    private void EndBattle() {
        isCleared = true;
        //SceneDoorScanner.Instance.ToggleRoomDoors(this, true);

        // ����ȫ�����ţ���������ѡ��
        SceneDoorScanner.Instance.SetAllDoorsOpenState(true);
        TipManager.Instance.ShowTip("完成");
        BGMManager.Instance.PlayBGM(BGMManager.Instance.normalBGM);

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
        yield return new WaitForSeconds(0.2f);

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
