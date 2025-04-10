using UnityEngine;
using System.Collections;
using Pathfinding;

public class SceneAIManager : MonoBehaviour
{
    [Header("配置参数")]
    [SerializeField] private string enemyTag = "Enemy";    // 敌人标签
    [SerializeField] private string playerTag = "Player"; // 玩家标签
    [SerializeField] private float scanInterval = 0.3f;    // 扫描间隔（秒）

    private Transform playerTransform;
    private Coroutine scanRoutine;

    void Start()
    {
        InitializePlayerReference();
        StartScanning();
    }

    // 初始化玩家引用
    void InitializePlayerReference()
    {
        GameObject playerObj = GameObject.FindWithTag(playerTag);
        if (playerObj != null)
        {
            playerTransform = playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError($"未找到标签为 {playerTag} 的玩家对象！");
            enabled = false;
        }
    }

    // 启动协程扫描
    void StartScanning()
    {
        if (scanRoutine != null) StopCoroutine(scanRoutine);
        scanRoutine = StartCoroutine(ScanEnemiesRoutine());
    }

    // 扫描协程
    IEnumerator ScanEnemiesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(scanInterval);
            ProcessExistingEnemies();
        }
    }

    // 处理现有敌人
    void ProcessExistingEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        foreach (GameObject enemy in enemies)
        {
            AIDestinationSetter aiSetter = enemy.GetComponent<AIDestinationSetter>();
            if (aiSetter != null && aiSetter.target == null)
            {
                aiSetter.target = playerTransform;
                Debug.Log($"已为 {enemy.name} 设置目标");
            }
        }
    }

    // 当组件被禁用时
    void OnDisable()
    {
        if (scanRoutine != null)
        {
            StopCoroutine(scanRoutine);
            scanRoutine = null;
        }
    }
}
