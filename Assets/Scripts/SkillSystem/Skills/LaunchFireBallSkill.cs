using System.Collections;
using UnityEngine;

public class LaunchFireBallSkill : SkillBase {

    [SerializeField] private GameObject fireballPrefab;

    public override void Execute(SkillSystem.ExecutionContext context) {

        StartCoroutine(FireballRoutine(context));

    }

    private IEnumerator FireballRoutine(SkillSystem.ExecutionContext context) {

        var playerAttributes = PlayerAttributes.Instance;
        var cardData = context.cardData;
        Vector3 direction = context.direction.normalized;

        float parentScaleSign = Mathf.Sign(playerAttributes.PlayerTransform.localScale.x);
        direction.x *= parentScaleSign;
        direction.y *= parentScaleSign;

        var config = cardData.behaviorConfig.projectile;

        int fireballCount = config.projectileCount;
        float interval = config.interval;
        float speed = config.speed;
        int baseDamage = config.damage;
        float maxRange = config.maxRange;

        int bonusDamage = playerAttributes.AttackPowerIncreased;
        int finalDamage = baseDamage + bonusDamage / fireballCount;

        fireballPrefab = cardData.visualConfig.castEffect;

        for (int i = 0; i < fireballCount; i++) {

            GameObject fireball = Instantiate(
                fireballPrefab,
                playerAttributes.PlayerTransform.position,
                Quaternion.identity
            );

            fireball.AddComponent<FireballMover>().Initialize(direction, speed, maxRange, cardData, finalDamage);

            yield return new WaitForSeconds(interval);
        }
    }
}

public class FireballMover : MonoBehaviour {

    private Vector3 direction;
    private float speed;
    private float maxRange;
    private Vector3 startPosition;
    private CardDataBase cardData;
    private int damage;

    public void Initialize(Vector3 dir, float spd, float range, CardDataBase cardD, int fd) {

        direction = dir;
        speed = spd;
        maxRange = range;
        startPosition = transform.position;
        cardData = cardD;
        damage = fd;

    }

    private void Update() {
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(startPosition, transform.position) >= maxRange) {

            Explode();

        }
    }

    // 用 OnTriggerEnter 进行碰撞检测
    private void OnTriggerEnter2D(Collider2D other) {

        Debug.Log(other.name);

        if(other.CompareTag("Player") || other.CompareTag("Floor") ||
            other.CompareTag("Bullet") || other.CompareTag("Room") ||
            other.CompareTag("BirthRoom")
            )
            return;

        if(other.CompareTag("Enemy")) {

            EnemyProperty enemy = other.GetComponent<EnemyProperty>();

            if(enemy != null) {

                enemy.TakeDamage(damage);

            }

        }

        Explode();
    }

    private void Explode() {

        GameObject explosion = Instantiate(cardData.visualConfig.hitEffect, transform.position, Quaternion.identity);
        Destroy(explosion, 0.5f);

        Destroy(gameObject);

    }

}
