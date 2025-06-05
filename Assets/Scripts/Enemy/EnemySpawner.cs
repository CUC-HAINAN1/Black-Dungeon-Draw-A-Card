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
    [Serializable]
    public class WaveConfig
    {
        public GameObject[] enemyPrefabs;
        public int enemyCount;
        public float spawnInterval = 1f;
    }

    [Header("��������")]
    [SerializeField] public WaveConfig[] waves;
    [SerializeField] public Transform[] spawnPoints;

    [Header("敌人刷新特效")]

    [SerializeField] private GameObject spawnEffectPrefab;
    [SerializeField] private float spawnEffectOffset = 0.2f;

    public List<EnemyProperty> activeEnemies = new List<EnemyProperty>();
    public int currentWaveIndex = -1;
    public int currentWaveDeadEnemyCount = 0;
    // ����״̬����
    public bool AllWavesCleared { get; private set; }
    public event Action AllWavesClearedEvent;
    public bool IsActive { get; private set; }

    public void StartSpawning()
    {

        if (IsActive) return;
        IsActive = true;
        StartNextWave();

    }
    private IEnumerator SpawnWave(WaveConfig wave)
    {
        for (int i = 0; i < wave.enemyCount; i++) {
            var spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

            // �����������ѡ�����Ԥ����
            if (wave.enemyPrefabs.Length > 0) {

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
            else {
                CustomLogger.LogError($"��{currentWaveIndex}��δ���õ���Ԥ����");
            }


        }
    }

    private void StartNextWave() {
        currentWaveIndex++;
        currentWaveDeadEnemyCount = 0;
        
        if (currentWaveIndex >= waves.Length) {
            CustomLogger.LogError("��������Խ��");
            return;
        }
        StartCoroutine(SpawnWave(waves[currentWaveIndex]));
    }

    private void HandleEnemyDeath(EnemyProperty enemy) {

        currentWaveDeadEnemyCount++;
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
