using UnityEngine;

public class EnemyDeathCommander : MonoBehaviour
{
    [Header("扣血设置")]
    [SerializeField] private KeyCode damageKey = KeyCode.L; // 默认L键
    [SerializeField] private int massDamageValue =20;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [Header("调试设置")]
    [SerializeField] private KeyCode killKey = KeyCode.K; // 可自定义按键
    [SerializeField] private bool enableHotkey = true;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    void Update()
    {
        if (!enableHotkey) return;

        if (Input.GetKeyDown(killKey))
        {
            // 通过标签查找所有敌人（需确保敌人预制体标记为"Enemy"）
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                var prop = enemy.GetComponent<EnemyProperty>();
                if (prop != null)
                {
                    prop.KillInstantly();
                }
            }

            Debug.Log($"已消灭 {enemies.Length} 个敌人");
        }
        if (enableHotkey && Input.GetKeyDown(damageKey))
        {
            ApplyMassDamage();
        }
    }
#endif
    private void ApplyMassDamage()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int successCount = 0;

        foreach (var enemy in enemies)
        {
            var prop = enemy.GetComponent<EnemyProperty>();
            if (prop != null && prop.IsAlive())
            {
                prop.TakeDamage(massDamageValue);
                successCount++;

                // 显示扣血效果
                Debug.DrawRay(enemy.transform.position, Vector3.up * 2, Color.yellow, 1f);
            }
        }

        Debug.Log($"成功对 {successCount} 个敌人造成{massDamageValue}点伤害");
    }
#endif
}
