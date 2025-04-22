using System.Collections;
using UnityEngine;

public class LightningSkill : SkillBase {

    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private float offsetY = 2f;

    public override void Execute(SkillSystem.ExecutionContext context) {

        StartCoroutine(LockOnTargetRoutine(context));

    }

    private IEnumerator LockOnTargetRoutine(SkillSystem.ExecutionContext context) {

        if (context.target == null) yield break;

        var cardData = context.cardData;
        var playerAttributes = PlayerAttributes.Instance;

        // 取出锁定的敌人目标
        Transform targetTransform = context.target.transform;

        Vector3 FinalPosition = targetTransform.position;

        FinalPosition.y += offsetY;

        var config = cardData.behaviorConfig.lockOn;
        float castDelay = config.delay;
        int baseDamage = config.damage;
        int bonusDamage = playerAttributes.AttackPowerIncreased;
        int finalDamage = baseDamage + bonusDamage;

        effectPrefab = cardData.visualConfig.castEffect;

        GameObject effectInstance = null;

        if (effectPrefab != null) {

            effectInstance = Instantiate(effectPrefab, FinalPosition , Quaternion.identity);

        }

        // 等待关键帧时间
        yield return new WaitForSeconds(castDelay);

        CameraShaker.Instance.ShakeOnce(1f, 10, 0.3f, 0.2f);

        // 如果目标还活着，造成伤害
        if (targetTransform != null && targetTransform.gameObject.activeInHierarchy) {

            if (targetTransform.gameObject.CompareTag("Enemy")) {

                EnemyProperty enemy = targetTransform.gameObject.GetComponentInChildren<EnemyProperty>();

                if (enemy != null) {

                    enemy.TakeDamage(finalDamage);

                }
            }

            if (targetTransform.CompareTag("Boss")) {

                BossHealth Boss = targetTransform.GetComponent<BossHealth>();

                if (Boss != null) {

                    Boss.TakeDamage(finalDamage);

                }
            }

        }

        // 延迟一点再销毁自身，确保特效完整播放
        if (effectInstance != null) {

            Destroy(effectInstance, 0.3f);

        }
    }
}
