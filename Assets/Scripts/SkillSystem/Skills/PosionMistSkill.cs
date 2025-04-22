using System.Collections;
using UnityEngine;
using System;

public class PoisonMistSkill : SkillBase {

    [SerializeField] private GameObject poisonMistPrefab;

    public override void Execute(SkillSystem.ExecutionContext context) {

        StartCoroutine(PoisonMistRoutine(context));

    }

    private IEnumerator PoisonMistRoutine(SkillSystem.ExecutionContext context) {

        var playerAttributes = PlayerAttributes.Instance;
        var cardData = context.cardData;
        Vector3 targetPosition = context.position;

        var config = cardData.behaviorConfig.area;

        float duration = config.duration;
        float interval = config.tickRate;
        float radius = config.radius;

        //增益伤害均摊到单次伤害
        int bonusDamage = System.Convert.ToInt32(playerAttributes.AttackPowerIncreased / (duration / interval));
        int damage = config.damage + bonusDamage;

        poisonMistPrefab = cardData.visualConfig.castEffect;

        GameObject poisonMist = Instantiate(poisonMistPrefab, targetPosition, Quaternion.identity);
        poisonMist.AddComponent<PoisonMistArea>().Initialize(duration, interval, radius, damage, cardData, poisonMist);

        yield return null;

    }

}

public class PoisonMistArea : MonoBehaviour {

    private float duration;
    private float interval;
    private float radius;
    private int damage;
    private GameObject posionMist;

    public void Initialize(float dur, float intvl, float rad, int dmg, CardDataBase data, GameObject posion) {

        duration = dur;
        interval = intvl;
        radius = rad;
        damage = dmg;
        posionMist = posion;

        StartCoroutine(DamageOverTime());
    }

    private IEnumerator DamageOverTime() {

        float elapsed = 0f;

        while (elapsed < duration) {

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radius * 3);

            foreach (var enemyCol in hitEnemies) {

                if (enemyCol.CompareTag("Enemy")) {

                    EnemyProperty enemy = enemyCol.GetComponentInChildren<EnemyProperty>();

                    if (enemy != null) {

                        enemy.TakeDamage(damage);

                    }

                }

                if (enemyCol.CompareTag("Boss")) {

                    BossHealth boss = enemyCol.GetComponent<BossHealth>();

                    if(boss != null) {

                        boss.TakeDamage(damage);

                    }

                }

            }

            elapsed += interval;
            yield return new WaitForSeconds(interval);
        }

        ExplodeAndDestroy();
    }

    private void ExplodeAndDestroy() {

        //GameObject hitEffect = Instantiate(cardData.visualConfig.hitEffect, transform.position, Quaternion.identity);
        //Destroy(hitEffect, 0.5f);
        Destroy(gameObject);
        Destroy(posionMist);

    }

}
