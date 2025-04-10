using UnityEngine;
using System.Collections;
public class BirthRoomTeleporter : MonoBehaviour
{
    [Header("调试设置")]
    [SerializeField] private bool showSpawnGizmos = true;
    [SerializeField] private Color gizmoColor = Color.green;

    void Start()
    {
        // 确保在第一个可执行帧之后执行
        StartCoroutine(TeleportWithDelay());
    }

    IEnumerator TeleportWithDelay()
    {
        yield return new WaitForEndOfFrame(); // 等待场景完全加载

        // 查找出生标记
        GameObject spawnAnchor = GameObject.FindWithTag("BirthRoom");
        if (spawnAnchor == null)
        {
            Debug.LogError("未找到出生标记！请检查：\n1.是否存在带BirthRoom标签的对象\n2.标签拼写是否正确");
            yield break;
        }

        // 查找玩家
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("未找到玩家！请检查：\n1.玩家对象是否存在\n2.是否设置Player标签");
            yield break;
        }

        // 执行传送
        player.transform.position = spawnAnchor.transform.position;
        DebugTeleportLog(spawnAnchor.transform.position);
    }

    void DebugTeleportLog(Vector3 pos)
    {
        Debug.Log($"玩家已出生在坐标：X:{pos.x:F2}, Y:{pos.y:F2}, Z:{pos.z:F2}");

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
