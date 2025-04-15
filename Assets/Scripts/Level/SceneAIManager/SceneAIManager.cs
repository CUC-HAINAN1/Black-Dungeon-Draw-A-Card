using UnityEngine;
using System.Collections;
using Pathfinding;

public class SceneAIManager : MonoBehaviour
{
    [Header("���ò���")]
    [SerializeField] private string enemyTag = "Enemy";    // ���˱�ǩ
    [SerializeField] private string playerTag = "Player"; // ��ұ�ǩ
    [SerializeField] private float scanInterval = 0.3f;    // ɨ�������룩

    private Transform playerTransform;
    private Coroutine scanRoutine;

    void Start()
    {
        InitializePlayerReference();
        StartScanning();
    }

    // ��ʼ���������
    void InitializePlayerReference()
    {
        GameObject playerObj = GameObject.FindWithTag(playerTag);
        if (playerObj != null)
        {
            playerTransform = playerTransform = playerObj.transform;
        }
        else
        {
            CustomLogger.LogError($"δ�ҵ���ǩΪ {playerTag} ����Ҷ���");
            enabled = false;
        }
    }

    // ����Э��ɨ��
    void StartScanning()
    {
        if (scanRoutine != null) StopCoroutine(scanRoutine);
        scanRoutine = StartCoroutine(ScanEnemiesRoutine());
    }

    // ɨ��Э��
    IEnumerator ScanEnemiesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(scanInterval);
            ProcessExistingEnemies();
        }
    }

    // �������е���
    void ProcessExistingEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        foreach (GameObject enemy in enemies)
        {
            AIDestinationSetter aiSetter = enemy.GetComponent<AIDestinationSetter>();
            if (aiSetter != null && aiSetter.target == null)
            {
                aiSetter.target = playerTransform;
                CustomLogger.Log($"��Ϊ {enemy.name} ����Ŀ��");
            }
        }
    }

    // �����������ʱ
    void OnDisable()
    {
        if (scanRoutine != null)
        {
            StopCoroutine(scanRoutine);
            scanRoutine = null;
        }
    }
}
