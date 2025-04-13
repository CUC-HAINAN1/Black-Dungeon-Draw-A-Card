using UnityEngine;
using UnityEngine.UI;

public class BossAI : MonoBehaviour
{
    [Header("距离设置")]
    public float meleeRange = 3f;
    public float rangedRange = 8f;

    [Header("技能设置")]
    public float skillCooldown = 2f;
    public GameObject projectilePrefab;
    public GameObject aoeIndicator;
    public float dashSpeed = 15f;
    public float dashDuration = 0.3f;

    public int damage=10;

    private Transform player;
    private bool isCooldown;
    private Rigidbody2D rb;

    
    public event System.Action<int> OnAOEPhaseChanged;
    public bool IsSkillActive => isCooldown;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isCooldown)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= meleeRange)
            {
                PerformMeleeAttack();
            }
            else if (distance <= rangedRange)
            {
                PerformRangedAttack();
            }
            else
            {
                PerformAOEAttack();
            }

            StartCoroutine(SkillCooldown());
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

    void PerformRangedAttack()
    {   
        GetComponent<BossAnimationController>().PlayAttackAnimation(2);
        // 发射远程弹幕
        Vector2 shootDirection = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().velocity = shootDirection * 10f;
        Destroy(projectile, 3f);
    }

    void PerformAOEAttack()
    {
        // 范围AOE攻击
        StartCoroutine(AOEAttackRoutine());
    }

    private System.Collections.IEnumerator AOEAttackRoutine()
    {    
        OnAOEPhaseChanged?.Invoke(1);
        // 显示预警区域
        GameObject indicator = Instantiate(aoeIndicator, player.position, Quaternion.identity);
        yield return new WaitForSeconds(1.5f);
        Destroy(indicator);


        yield return new WaitForSeconds(1.5f);
        OnAOEPhaseChanged?.Invoke(2); // 下砸阶段
        // 实际伤害判定
        float aoeRadius = 4f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(player.position, aoeRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                // 对玩家造成伤害
                PlayerAttributes.Instance.TakeDamage(damage);
            }
        }
    }
}