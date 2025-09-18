using UnityEngine;
using System.Collections;
public class BirthRoomTeleporter : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private bool showSpawnGizmos = true;
    [SerializeField] private Color gizmoColor = Color.green;

    void Start()
    {
        // ȷ���ڵ�һ����ִ��֮֡��ִ��
        StartCoroutine(TeleportWithDelay());
    }

    IEnumerator TeleportWithDelay()
    {
        yield return new WaitForEndOfFrame(); // �ȴ�������ȫ����

        // ���ҳ������
        GameObject spawnAnchor = GameObject.FindWithTag("BirthRoom");
        if (spawnAnchor == null)
        {
            Debug.LogError("δ�ҵ�������ǣ����飺\n1.�Ƿ���ڴ�BirthRoom��ǩ�Ķ���\n2.��ǩƴд�Ƿ���ȷ");
            yield break;
        }

        // �������
        GameObject player = GameObject.FindWithTag("Player");

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            Debug.LogWarning($"Before Spawning, players name: {p.name}");
        }


        if (player == null) {
                Debug.LogError("δ�ҵ���ң����飺\n1.��Ҷ����Ƿ����\n2.�Ƿ�����Player��ǩ");
                yield break;
            }

        Debug.LogWarning($"Before Spawning: Player Transform: {player.transform.position}");
        // ִ�д���
        player.transform.position = spawnAnchor.transform.position;

        Debug.LogWarning($"Spawn player to birth room. Transform: {player.transform.position}");
        DebugTeleportLog(spawnAnchor.transform.position);
    }

    void DebugTeleportLog(Vector3 pos)
    {
        Debug.Log($"����ѳ��������꣺X:{pos.x:F2}, Y:{pos.y:F2}, Z:{pos.z:F2}");

        if (showSpawnGizmos)
        {
            Debug.DrawRay(pos, Vector3.up * 2, gizmoColor, 5f);
        }
    }

    void OnDrawGizmos()
    {
        if (!showSpawnGizmos) return;

        var spawn = GameObject.FindWithTag("BirthRoom");
        if (spawn != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireCube(spawn.transform.position, new Vector3(1, 1, 0));
            Gizmos.DrawIcon(spawn.transform.position + Vector3.up * 0.5f, "spawn_icon");
        }
    }
}
