using UnityEngine;

public class EnemyDeathCommander : MonoBehaviour
{
    [Header("��Ѫ����")]
    [SerializeField] private KeyCode damageKey = KeyCode.L; // Ĭ��L��
    [SerializeField] private int massDamageValue =20;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [Header("��������")]
    [SerializeField] private KeyCode killKey = KeyCode.K; // ���Զ��尴��
    [SerializeField] private bool enableHotkey = true;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    void Update()
    {
        if (!enableHotkey) return;

        if (Input.GetKeyDown(killKey))
        {
            // ͨ����ǩ�������е��ˣ���ȷ������Ԥ������Ϊ"Enemy"��
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                var prop = enemy.GetComponent<EnemyProperty>();
                if (prop != null)
                {
                    prop.KillInstantly();
                }
            }

            Debug.Log($"������ {enemies.Length} ������");
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

                // ��ʾ��ѪЧ��
                Debug.DrawRay(enemy.transform.position, Vector3.up * 2, Color.yellow, 1f);
            }
        }

        Debug.Log($"�ɹ��� {successCount} ���������{massDamageValue}���˺�");
    }
#endif
}
