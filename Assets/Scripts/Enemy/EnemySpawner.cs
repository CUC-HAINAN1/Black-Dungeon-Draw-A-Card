using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemySpawner : MonoBehaviour
{
    [Header("房间适配")]
    [SerializeField] private bool autoAdjustSpawnPoints = true;
    public Vector2 GetEdgarAdjustedPosition(Vector2 rawPosition)
    {
        // 适配截图中的2000坐标系和Scale=2参数
        return new Vector2(
            Mathf.Round(rawPosition.x / 1000f) * 2f,
            Mathf.Round(rawPosition.y / 1000f) * 2f
        );
    }
    public void SyncWithDoors(DoorStateController[] doors)
    {
        foreach (var door in doors)
        {
            // 根据截图中的Reset方向参数确保门朝向正确
            door.transform.rotation = Quaternion.identity;
        }
    }
    [System.Serializable]
    public class WaveConfig
    {
        public GameObject[] enemyPrefabs; // 改为复数形式表示数组
        public int enemyCount;
        public float spawnInterval = 1f;
    }

    [Header("生成设置")]
    [SerializeField] private WaveConfig[] waves;
    [SerializeField] public Transform[] spawnPoints;

    public List<EnemyProperty> activeEnemies = new List<EnemyProperty>();
    private int currentWaveIndex = -1;
    // 新增状态属性
    public bool AllWavesCleared { get; private set; }
    public event Action AllWavesClearedEvent;
    public bool IsActive { get; private set; }
    private void HandleWaveCompletion()
    {
        if (currentWaveIndex >= waves.Length - 1 && activeEnemies.Count == 0)
        {
            AllWavesCleared = true;
            AllWavesClearedEvent?.Invoke();
        }
    }
    public void StartSpawning()
    {
       
        if (IsActive) return;
        IsActive = true;
        StartNextWave();

    }
    private IEnumerator SpawnWave(WaveConfig wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            var spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

            // 从数组中随机选择敌人预制体
            if (wave.enemyPrefabs.Length > 0)
            {
                GameObject randomEnemy = wave.enemyPrefabs[UnityEngine.Random.Range(0, wave.enemyPrefabs.Length)];
                var enemy = Instantiate(randomEnemy, spawnPoint.position, Quaternion.identity);
                var property = enemy.GetComponent<EnemyProperty>();
                property.OnDeath.AddListener(HandleEnemyDeath);
                activeEnemies.Add(property);
            }
            else
            {
                Debug.LogError($"第{currentWaveIndex}波未配置敌人预制体");
            }

            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    private void StartNextWave()
    {
        currentWaveIndex++;
        if (currentWaveIndex >= waves.Length)
        {
            Debug.LogError("波次索引越界");
            return;
        }
        StartCoroutine(SpawnWave(waves[currentWaveIndex]));
    }

    bool IsPositionValid(Vector3 pos)
    {
        // 获取房间碰撞器边界
        var roomCollider = GetComponentInParent<BoxCollider2D>();
        return roomCollider.bounds.Contains(pos);
    }
    // 在EnemySpawner脚本中添加验证逻辑
    void ValidateSpawnPoints()
    {
        // 获取层级结构中所有标记为SpawnPoints的子对象
        var points = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.name.Contains("SpawnPoint")) // 匹配你的"SpawnPoints"层级
                points.Add(child);
        }
        spawnPoints = points.ToArray();

        // 打印调试信息
        Debug.Log($"已绑定生成点数量: {spawnPoints.Length}");
        foreach (var point in spawnPoints)
            Debug.Log($"生成点坐标: {point.position}");
    }
    private void HandleEnemyDeath(EnemyProperty enemy)
    {
        activeEnemies.Remove(enemy);
        enemy.OnDeath.RemoveListener(HandleEnemyDeath); // 重要：解除事件绑定
                                                        // 新增波次检测
        if (activeEnemies.Count == 0)
        {
            if (currentWaveIndex < waves.Length - 1)
            {
                // 自动开始下一波
                StartNextWave();
            }
            else
            {
                // 最终波次完成
                AllWavesCleared = true;
                AllWavesClearedEvent?.Invoke();
            }
        }
       
    }

    public void KillAllEnemies()
    {
        foreach (var enemy in activeEnemies.ToArray())
        {
            enemy.InstantDie();
        }
    }
}
