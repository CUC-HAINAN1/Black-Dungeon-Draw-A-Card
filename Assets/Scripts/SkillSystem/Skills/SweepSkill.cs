using UnityEngine;
using System.Collections;

public class SweepSkill : SkillBase {
    [SerializeField] private GameObject sectorPrefab;

    public override void Execute(SkillSystem.ExecutionContext context) {

        var playerAttributes = PlayerAttributes.Instance;
        var cardData = context.cardData;
        Vector3 direction = context.direction.normalized;

        float parentScaleSign = Mathf.Sign(playerAttributes.PlayerTransform.localScale.x);
        direction.x *= parentScaleSign;
        direction.y *= parentScaleSign;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        var config = cardData.behaviorConfig.sweep;
        int baseDamage = config.damage;
        int bonusDamage = playerAttributes.AttackPowerIncreased;
        int finalDamage = baseDamage + bonusDamage;

        sectorPrefab = cardData.visualConfig.castEffect;

        Vector3 spawnPos = playerAttributes.PlayerTransform.position;
        spawnPos += direction * 0.5f;

        GameObject sectorEffect = Instantiate(
            sectorPrefab,
            spawnPos,
            Quaternion.Euler(0, 0, angle),
            playerAttributes.PlayerTransform
        );

        // SectorHitbox :延迟伤害 + 判定 + 自销毁
        var sector = sectorEffect.AddComponent<SectorHitbox>();
        sector.Initialize(
            playerAttributes.PlayerTransform.position,
            direction,
            config.radius,
            config.angle,
            finalDamage,
            config.delay
        );

    }
}

public class SectorHitbox : MonoBehaviour {

    private Vector2 origin;
    private Vector2 direction;
    private float radius;
    private float angle;
    private int damage;
    private float delay;

    public void Initialize(Vector2 ori, Vector2 dir, float rad, float ang, int dmg, float delayTime) {
        origin = ori;
        direction = dir.normalized;
        radius = rad;
        angle = ang;
        damage = dmg;
        delay = delayTime;

        StartCoroutine(DelayAndHit());
    }

    private IEnumerator DelayAndHit() {

        yield return new WaitForSeconds(delay);

        Vector2 currentOrigin = transform.position;
        Collider2D[] hitEnemys = Physics2D.OverlapCircleAll(currentOrigin, radius * 5);
        float halfAngle = angle * 0.5f;

        foreach (var enemyCol in hitEnemys) {

            if (!enemyCol.CompareTag("Enemy"))
                continue;

            //扇形区域检测
            Vector2 toTarget = (Vector2)enemyCol.transform.position - origin;

            if (toTarget.sqrMagnitude == 0)
                continue;

            float angleBetween = Vector2.Angle(direction, toTarget.normalized);
            if (angleBetween <= halfAngle) {

                 EnemyProperty enemy = enemyCol.GetComponent<EnemyProperty>();

                 if (enemy != null) {

                     enemy.TakeDamage(damage);

                 }

            }
        }

        // 延迟一点再销毁自身，确保特效完整播放
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);

    }

}

