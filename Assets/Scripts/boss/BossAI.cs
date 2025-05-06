using UnityEngine;
using UnityEngine.UI;

public class BossAI : MonoBehaviour
{
    [Header("距离设置")]
    public float meleeRange = 3f;
    public float rangedRange = 8f;
    public float aoeRadius = 3f;

    [Header("技能设置")]
    public float skillCooldown;
    public GameObject projectilePrefab;
    public GameObject aoeIndicator;
    public GameObject aoeEffect;
    public float dashSpeed = 15f;
    public float dashDuration = 0.3f;

    public int damage = 20;

    private Transform player;
    private bool isCooldown;
    private Rigidbody2D rb;

    public event System.Action<int> OnAOEPhaseChanged;
    public bool IsSkillActive => isCooldown;

    [Header("随机移动设置")]
    public float moveSpeed;
    public float moveDuration = 1f;
    public float idleDuration = 2f;

    [Range(0f, 1f)]
    public float moveChance = 0.4f; // 每次判断有 40% 概率移动

    private Vector2 moveDirection;
    private float moveTimer;
    private float idleTimer;
    private bool isMoving = false;

    [Header("Boss数据")]
    [SerializeField] public BossData bossData;

    void Start() {

        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        BGMManager.Instance.PlayBGM(BGMManager.Instance.BossBGM);

        StartCoroutine(SkillCooldown());
        idleTimer = idleDuration;

        skillCooldown = bossData.skillCooldown;
        moveSpeed = bossData.moveSpeed;

    }

    void Update() {

        if (!isCooldown) {

            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= meleeRange) {
                PerformMeleeAttack();
            }
            else if (distance <= rangedRange) {
                PerformRangedAttack();
            }
            else {
                PerformAOEAttack();
            }

            StartCoroutine(SkillCooldown());

        }

        if (isMoving)
        {
            rb.velocity = moveDirection * moveSpeed;
            moveTimer -= Time.deltaTime;
            if (moveTimer <= 0f)
            {
                isMoving = false;
                rb.velocity = Vector2.zero;
                idleTimer = idleDuration;
            }
        }
        else
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                if (Random.value < moveChance)
                {
                    // 开始新一轮移动
                    moveDirection = Random.insideUnitCircle.normalized;
                    isMoving = true;
                    moveTimer = moveDuration;
                }
                else
                {
                    // 再等一会再试
                    idleTimer = idleDuration;
                }
            }
        }
    }

    private System.Collections.IEnumerator SkillCooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(skillCooldown);
        isCooldown = false;
    }

    void PerformMeleeAttack()
    {
        GetComponent<BossAnimationController>().PlayAttackAnimation(1);
        // 冲刺攻击
        Vector2 dashDirection = (player.position - transform.position).normalized;
        StartCoroutine(Dash(dashDirection));
    }

    private System.Collections.IEnumerator Dash(Vector2 direction)
    {
        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            rb.velocity = direction * dashSpeed;
            yield return null;
        }

        rb.velocity = Vector2.zero;
    }

    void PerformRangedAttack() {

        GetComponent<BossAnimationController>().PlayAttackAnimation(2);
        // 发射远程弹幕
        Vector2 shootDirection = (player.position - transform.position).normalized;

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle));

        projectile.GetComponent<Rigidbody2D>().velocity = shootDirection * 10f;
        Destroy(projectile, 3f);

    }

    void PerformAOEAttack()
    {
        // 范围AOE攻击
        StartCoroutine(AOEAttackRoutine());
    }

    private System.Collections.IEnumerator AOEAttackRoutine() {
        OnAOEPhaseChanged?.Invoke(1);
        // 显示预警区域

        Vector3 pos = player.position;

        GameObject indicator = Instantiate(aoeIndicator, pos, Quaternion.identity);

        SpriteRenderer sr = indicator.GetComponentInChildren<SpriteRenderer>();
        float spriteWorldSize = sr.sprite.bounds.size.x;

        float targetScale = aoeRadius * 2 / spriteWorldSize;
        indicator.transform.localScale = new Vector3(targetScale, targetScale, 1f);

        yield return new WaitForSeconds(1.5f);
        Destroy(indicator);

        Vector3 effectPos = pos;
        effectPos.y += 2.5f;

        GameObject effect = Instantiate(aoeEffect, effectPos, Quaternion.identity);
        effect.transform.localScale = new Vector3(targetScale / 2.5f, targetScale / 2.5f, 1f);

        CameraShaker.Instance.ShakeOnce(2f, 10, 0.6f, 0.3f);

        yield return new WaitForSeconds(0.25f);

        OnAOEPhaseChanged?.Invoke(2); // 下砸阶段
        // 实际伤害判定
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, aoeRadius);

        foreach (Collider2D hit in hits) {

            if (hit.CompareTag("Player")) {

                // 对玩家造成伤害
                PlayerAttributes.Instance.TakeDamage(damage);

            }

        }

        yield return new WaitForSeconds(0.5f);
        Destroy(effect);

    }
}
