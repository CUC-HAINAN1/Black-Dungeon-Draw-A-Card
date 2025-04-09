using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class EnemyProperty : MonoBehaviour
{
    [Header("基础属性")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2f;

    [Header("巡逻设置")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waitDuration = 1f;

    [Header("死亡设置")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private float destroyDelay = 2f;

    // 事件
    public UnityEvent<EnemyProperty> OnDeath;
    public UnityEvent<int> OnHealthChanged;

    private int currentHealth;
    private float lastAttackTime;
    private int currentPatrolIndex;
    private bool isPatrolling;

    // 血条UI组件
    private EnemyHealthUI healthUI;

    void Awake()
    {
        currentHealth = maxHealth;
        healthUI = GetComponentInChildren<EnemyHealthUI>();
        healthUI?.Initialize(maxHealth);
    }

    void Start()
    {
        StartPatrol();
    }

    void Update()
    {
        if (isPatrolling)
        {
            PatrolMovement();
        }
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive()) return; // 防止重复伤害

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
        healthUI?.UpdateHealth(currentHealth);


        if (currentHealth <= 0)
        {
            Die();
        }
    }
    [Header("受击效果")]
    [SerializeField] private ParticleSystem hitEffect;

    private void Die()
    {
        OnDeath?.Invoke(this);
        StopAllCoroutines();
        GetComponent<Collider2D>().enabled = false;

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        healthUI?.Hide();
        Destroy(gameObject, destroyDelay);
    }

    private void StartPatrol()
    {
        if (patrolPoints.Length < 2) return;
        isPatrolling = true;
        currentPatrolIndex = 0;
    }

    private void PatrolMovement()
    {
        Transform target = patrolPoints[currentPatrolIndex];
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            patrolSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            StartCoroutine(WaitAtPoint());
        }
    }
    public void KillInstantly()
    {
        // 直接终止所有流程
        currentHealth = 0;
        StopAllCoroutines();
        isPatrolling = false;

        // 触发完整死亡流程
        healthUI?.UpdateHealth(0);
        Die();

        // 立即隐藏对象
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }
    private IEnumerator WaitAtPoint()
    {
        isPatrolling = false;
        yield return new WaitForSeconds(waitDuration);
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        isPatrolling = true;
    }
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
    public void InstantDie()
    {
        TakeDamage(maxHealth);
        KillInstantly();
    }
}
