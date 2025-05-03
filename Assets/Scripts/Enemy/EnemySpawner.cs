using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemySpawner : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private bool autoAdjustSpawnPoints = true;
    public Vector2 GetEdgarAdjustedPosition(Vector2 rawPosition)
    {
        // �����ͼ�е�2000����ϵ��Scale=2����
        return new Vector2(
            Mathf.Round(rawPosition.x / 1000f) * 2f,
            Mathf.Round(rawPosition.y / 1000f) * 2f
        );
    }
    public void SyncWithDoors(DoorStateController[] doors)
    {
        foreach (var door in doors)
        {
            // ���ݽ�ͼ�е�Reset�������ȷ���ų�����ȷ
            door.transform.rotation = Quaternion.identity;
        }
    }
    [System.Serializable]
    public class WaveConfig
    {
        public GameObject[] enemyPrefabs;
        public int enemyCount;
        public float spawnInterval = 1f;
    }

    [Header("��������")]
    [SerializeField] private WaveConfig[] waves;
    [SerializeField] public Transform[] spawnPoints;

    [Header("敌人刷新特效")]

    [SerializeField] private GameObject spawnEffectPrefab;
    [SerializeField] private float spawnEffectOffset = 0.2f;

    public List<EnemyProperty> activeEnemies = new List<EnemyProperty>();
    private int currentWaveIndex = -1;
    // ����״̬����
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

            // �����������ѡ�����Ԥ����
            if (wave.enemyPrefabs.Length > 0)
            {

                if (spawnEffectPrefab != null) {

                    var effectPos = spawnPoint.position;
                    effectPos.y -= spawnEffectOffset;

                    var effect = Instantiate(spawnEffectPrefab, effectPos, Quaternion.identity);

                    yield return new WaitForSeconds(0.75f);

                    Destroy(effect);

                }

                GameObject randomEnemy = wave.enemyPrefabs[UnityEngine.Random.Range(0, wave.enemyPrefabs.Length)];
                var enemy = Instantiate(randomEnemy, spawnPoint.position, Quaternion.identity);

                if (enemy.CompareTag("Boss")) {

                    continue;

                }
                var property = enemy.GetComponentInChildren<EnemyProperty>();
                property.OnDeath.AddListener(HandleEnemyDeath);
                activeEnemies.Add(property);
            }
            else
            {
                CustomLogger.LogError($"��{currentWaveIndex}��δ���õ���Ԥ����");
            }

            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    private IEnumerator SpawnEffect(Transform SpawnTrans) {

        var effect = Instantiate(spawnEffectPrefab, SpawnTrans.position, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);

        Destroy(effect);

    }

    private void StartNextWave() {
        currentWaveIndex++;
        if (currentWaveIndex >= waves.Length) {
            CustomLogger.LogError("��������Խ��");
            return;
        }
        StartCoroutine(SpawnWave(waves[currentWaveIndex]));
    }

    bool IsPositionValid(Vector3 pos)
    {
        // ��ȡ������ײ���߽�
        var roomCollider = GetComponentInParent<BoxCollider2D>();
        return roomCollider.bounds.Contains(pos);
    }

    // ��EnemySpawner�ű���������֤�߼�
    void ValidateSpawnPoints()
    {
        // ��ȡ�㼶�ṹ�����б��ΪSpawnPoints���Ӷ���
        var points = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.name.Contains("SpawnPoint")) // ƥ�����"SpawnPoints"�㼶
                points.Add(child);
        }
        spawnPoints = points.ToArray();

        // ��ӡ������Ϣ
        CustomLogger.Log($"�Ѱ����ɵ�����: {spawnPoints.Length}");
        foreach (var point in spawnPoints)
            CustomLogger.Log($"���ɵ�����: {point.position}");
    }

    private void HandleEnemyDeath(EnemyProperty enemy) {
        activeEnemies.Remove(enemy);
        enemy.OnDeath.RemoveListener(HandleEnemyDeath); // ��Ҫ������¼���
                                                        // �������μ��
        if (activeEnemies.Count == 0) {
            if (currentWaveIndex < waves.Length - 1) {
                // �Զ���ʼ��һ��
                StartNextWave();
            }
            else {
                // ���ղ������
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
