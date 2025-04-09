using Edgar.Unity;
using Pathfinding.Examples;
using UnityEngine;
using System.Collections.Generic;
using Google.Events.Protobuf.Cloud.NetworkManagement.V1;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomManager : MonoBehaviour
{ // 新增状态属性

    [Header("战斗配置")]
    [SerializeField] private bool isCombatRoom; // 在Edgar模板中设置
    [SerializeField] private EnemySpawner enemySpawner;

    [Header("房间尺寸")]
    [SerializeField] public Vector2 roomSize = new Vector2(11, 11); // 手动设置尺寸


    [Header("门控制")]
    [SerializeField] public DoorStateController[] doors;
    [Header("房间状态")]
    [SerializeField] private bool isCleared; // 新增房间清理状态标记

    private bool hasActiveEnemies;
    private BoxCollider2D roomTrigger;
    private bool isRoomCleared; // 新增房间状态标记

    [ContextMenu("打印生成器状态")]
    void DebugSpawnerStatus()
    {
        if (enemySpawner == null)
            Debug.LogError("EnemySpawner未绑定");
        else
            Debug.Log($"已绑定生成器，包含{enemySpawner.spawnPoints.Length}个生成点");
    }
    void AutoFindDoors()
    {
        var doorList = new List<DoorStateController>();
        // 方案2：通过标签查找
        GameObject[] doorObjs = GameObject.FindGameObjectsWithTag("Door");
        foreach (var obj in doorObjs)
        {
            if (obj.transform.IsChildOf(transform)) // 确保是本房间的子对象
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
            door.InitDoorState(); // 确保门方向正确
        }
        // 适配Edgar的11x11房间尺寸
        roomTrigger = GetComponent<BoxCollider2D>();
        roomTrigger.size = roomSize;
        roomTrigger.isTrigger = true;

        // 自动适配Edgar生成的门（对应VerDownDoor前缀）
        doors = GetComponentsInChildren<DoorStateController>(true);
    }


    public void CloseAllDoors()
    {
        foreach (var door in doors)
        {
            // 适配Edgar的Scale=2参数
            door.transform.localScale = Vector3.one;
            door.CloseDoor();
        
        }
    }

    void StartEnemySpawning()
    {
        if (enemySpawner != null)
        {
            // 适配Edgar的Scale参数
            enemySpawner.transform.localScale = Vector3.one * 2;

            // 调整生成点坐标
            foreach (Transform point in enemySpawner.spawnPoints)
            {
                point.position = enemySpawner.GetEdgarAdjustedPosition(point.position);
            }
            enemySpawner.StartSpawning();
            enemySpawner.AllWavesClearedEvent += OnEnemiesCleared;
            // 同步门状态
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
        // 改用事件驱动检测
        return enemySpawner != null && enemySpawner.activeEnemies.Count > 0;
    }

    void OnEnemiesCleared()
    {
        isCleared = true; // 标记房间为已清理
        isRoomCleared = true; // 持久化状态
        hasActiveEnemies = false;
        OpenAllDoors();
        // 触发全局系统更新（适配截图中的2000坐标范围）
        GlobalRoomSystem.UpdateConnectedRooms(transform.position * 1000f);
    }

    void OpenAllDoors()
    {
        foreach (var door in doors)
        {
            door.OpenDoor();
        }
    }

    // Edgar坐标转换（适配2000坐标系）
    public Vector2 GetGridPosition()
    {
        return new Vector2(
            Mathf.RoundToInt(transform.position.x*1000 ),
            Mathf.RoundToInt(transform.position.y*1000 )
        );
    }
    // 在原有类中添加以下方法
    private void StartBattle()
    {
        if (!isCombatRoom || isCleared) return;

        // 关闭本房间所有门
        //SceneDoorScanner.Instance.ToggleRoomDoors(this, false);

        // 关闭全场景门（根据需求选择其中一种）
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

        // 开启全场景门（根据需求选择）
         SceneDoorScanner.Instance.SetAllDoorsOpenState(true);
    }

    // 修改原有OnTriggerEnter2D
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isCleared) return;

        StartCoroutine(DelayedDoorClose());
    }

    IEnumerator DelayedDoorClose()
    {
        // 保持开门状态等待玩家进入
        yield return new WaitForSeconds(0.2f); // 0.2秒缓冲时间

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
