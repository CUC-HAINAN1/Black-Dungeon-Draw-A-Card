using System.Collections;

using UnityEngine;

public class MeteoricFireballSkill : SkillBase {

    [SerializeField] private GameObject aoeEffectPrefab;

    public override void Execute(SkillSystem.ExecutionContext context) {
        StartCoroutine(MeteoricFireballRoutine(context));
    }

    private IEnumerator MeteoricFireballRoutine(SkillSystem.ExecutionContext context) {

        var playerAttributes = PlayerAttributes.Instance;
        var cardData = context.cardData;
        Vector3 targetPosition = context.position;
        targetPosition.y += 1.6f;

        var config = cardData.behaviorConfig.burstAOE;

        float radius = config.radius;

        int bonusDamage = playerAttributes.AttackPowerIncreased;
        int damage = config.damage + bonusDamage;
        float hitDelay = config.delay;

        aoeEffectPrefab = cardData.visualConfig.castEffect;

        GameObject aoeEffect = Instantiate(aoeEffectPrefab, targetPosition, Quaternion.identity);
        aoeEffect.AddComponent<MeteoricFireballExplosion>().Initialize(radius, damage, hitDelay, cardData, aoeEffect);

        yield return null;
    }

}
public class MeteoricFireballExplosion : MonoBehaviour {

    private float radius;
    private int damage;
    private float hitDelay;
    private CardDataBase cardData;
    private GameObject effectObject;

    public void Initialize(float rad, int dmg, float delay, CardDataBase data, GameObject effect) {

        radius = rad;
        damage = dmg;
        hitDelay = delay;
        cardData = data;
        effectObject = effect;

        StartCoroutine(ExplosionDelayRoutine());
    }

    private IEnumerator ExplosionDelayRoutine() {

        yield return new WaitForSeconds(hitDelay);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radius * 6f);

        Debug.Log($"实际攻击半径: {radius * 6f}");

        foreach (var enemyCol in hitEnemies) {

            Debug.Log(enemyCol.tag);

            if (enemyCol.CompareTag("Enemy")) {

                EnemyProperty enemy = enemyCol.GetComponent<EnemyProperty>();

                if (enemy != null) {
                    enemy.TakeDamage(damage);
                }

            }

        }

        yield return new WaitForSeconds(0.5f);

        Destroy(effectObject);
        Destroy(gameObject);
    }

}
