using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Pathfinding;
using System.Collections.Generic;

public class EnemyProperty : MonoBehaviour
{
    [Header("基础属性")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 2f;

    [Header("巡逻设置")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waitDuration = 1f;

    [Header("死亡设置")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private float destroyDelay = 2f;

    [Header("攻击配置")]
    [SerializeField] private Transform attackPoint;  // 攻击判定点
    [SerializeField] private float attackRadius = 1f; // 攻击半径
    [SerializeField] private LayerMask playerLayer; // 玩家层级
    [SerializeField] private EnemyAttackType attackType = EnemyAttackType.Melee;
    [SerializeField] private float detectionRange = 5f;    // 追击范围
    [SerializeField] private float attackRange = 2f;       // 近战攻击范围
    [SerializeField] private float projectileSpeed = 10f;   // 远程投射速度
    [SerializeField] private Transform weaponPivot;         // 武器旋转支点
    [SerializeField] private GameObject projectilePrefab;   // 远程投射物
    [SerializeField] private float attackInterval = 2f;     // 远程攻击间隔

    //混合攻击模式设置
    [Header("高级配置")]
    [SerializeField] private bool enableHybridAttack = false;
    [SerializeField] private float hybridRangeThreshold = 3f;

    [Header("Aseprite动画配置")]
    [SerializeField] private Sprite[] attackFrames;     // Aseprite导出的序列帧
    [SerializeField] private GameObject effectContainer; // 特效挂载点
    [SerializeField] private float frameRate = 12f;     // 每秒播放帧数
    [SerializeField] private bool loopAttackEffect;     // 是否循环播放

    private SpriteRenderer effectRenderer;
    private Coroutine attackEffectCoroutine;
    private int currentFrame;
    private void HandleHybridAttack()
    {
        if (!enableHybridAttack) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= hybridRangeThreshold)
        {
            PerformMeleeAttack();
        }
        else
        {
            PerformRangedAttack();
        }
    }
    // 统一使用AIState枚举
    private enum AIState { Patrolling, Chasing, Attacking }
    private AIState currentAIState;
    private Transform playerTransform;
    private bool canAttack = true;
    private float attackCooldownTimer;
    [System.Serializable]
    public enum EnemyAttackType
    {
        Melee,
        Ranged
    }


    public void PerformAttack()
    {
        if (!canAttack || !IsAlive()) return;

        // 检测攻击范围内的玩家
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            playerLayer
        );

        foreach (var player in hitPlayers)
        {
            if (player.CompareTag("Player"))
            {
                PlayerAttributes.Instance.TakeDamage(attackDamage);
                Debug.Log($"对玩家造成 {attackDamage} 点伤害");
            }
        }

        // 触发攻击冷却
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // 在Scene视图显示攻击范围（调试用）
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    // 事件
    public UnityEvent<EnemyProperty> OnDeath;
    public UnityEvent<int> OnHealthChanged;

    private int currentHealth;
    private float lastAttackTime;
    private int currentPatrolIndex;
    private bool isPatrolling;

    // 血条UI组件
    private EnemyHealthUI healthUI;

    [Header("动画控制")]
    [SerializeField] private Animator animator;

    [Header("状态管理")]
    [SerializeField] private EnemyStateMachine stateMachine;

    void Awake()
    {
        currentHealth = maxHealth;
        healthUI = GetComponentInChildren<EnemyHealthUI>();
        healthUI?.Initialize(maxHealth);
    }

    void Start()
    {
        StartPatrol();
        if (effectContainer == null)
        {
            effectContainer = new GameObject("AttackEffect");
            effectContainer.transform.SetParent(transform);
            effectContainer.transform.localPosition = Vector3.zero;
        }

        if (effectRenderer == null)
        {
            effectRenderer = effectContainer.AddComponent<SpriteRenderer>();
            effectRenderer.sortingOrder = 10; // 确保显示在角色上层
        }
        if (animator == null) animator = GetComponentInChildren<Animator>();
        playerTransform = PlayerAttributes.Instance.PlayerTransform;
        currentAIState = AIState.Patrolling;
        if (effectContainer != null)
        {
            effectRenderer = effectContainer.AddComponent<SpriteRenderer>();
            effectRenderer.enabled = false;
        }
    }
    // 攻击动画播放方法（通过条件直接调用）
    private void PlayAttackEffect()
    {
        if (attackEffectCoroutine != null) StopCoroutine(attackEffectCoroutine);
        attackEffectCoroutine = StartCoroutine(PlayFrameAnimation());
    }

    private IEnumerator PlayFrameAnimation()
    {
        effectRenderer.enabled = true;
        currentFrame = 0;

        while (currentFrame < attackFrames.Length)
        {
            effectRenderer.sprite = attackFrames[currentFrame];
            currentFrame++;
            yield return new WaitForSeconds(1 / frameRate);
        }

        effectRenderer.enabled = loopAttackEffect;
    }
    void Update()
    { // 根据移动状态更新参数
        if (!IsAlive()) return; // 原条件写反了

        UpdateAIState();
        HandleWeaponRotation();
        HandleAttackLogic();
        // 保留原有移动状态更新
        bool isMoving = GetComponent<AIPath>().velocity.magnitude > 0.1f;
        animator.SetBool("IsMoving", isMoving);

        if (isPatrolling)
        {
            PatrolMovement();
        }
    }
    //追击行为实现
    private void ChaseBehavior()
    {
        GetComponent<AIPath>().maxSpeed = patrolSpeed * 2;
        GetComponent<AIPath>().destination = playerTransform.position;
    }
    //巡逻系统优化
    private void PatrolBehavior()
    {
        if (!isPatrolling) return;

        Transform target = patrolPoints[currentPatrolIndex];
        Vector2 newPos = Vector2.MoveTowards(
            transform.position,
            target.position,
            patrolSpeed * Time.deltaTime
        );

        // 使用AIPath控制移动
        GetComponent<AIPath>().destination = target.position;
    }
    //状态转换管理
    private void UpdateAIState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // 添加状态切换保护
        if (currentAIState == AIState.Attacking && !canAttack) return;

        // 状态切换逻辑
        switch (currentAIState)
        {
            case AIState.Patrolling:
                PatrolBehavior();
                if (distanceToPlayer <= detectionRange)
                    TransitionToState(AIState.Chasing);
                break;

            case AIState.Chasing:
                ChaseBehavior();
                if (distanceToPlayer <= attackRange) TransitionToState(AIState.Attacking);
                else if (distanceToPlayer > detectionRange * 1.2f)
                    TransitionToState(AIState.Patrolling);
                break;

            case AIState.Attacking:
                if (distanceToPlayer > attackRange * 1.1f)
                    TransitionToState(AIState.Chasing);
                break;
        }
    }
    private void SetStateImmediate(AIState newState)
    {
        currentAIState = newState;
        animator.Play(newState.ToString(), 0, 0); // 强制重置动画状态
    }
    private void HandleAttackLogic()
    {
        if (currentAIState != AIState.Attacking || !canAttack) return;

        switch (attackType)
        {
            case EnemyAttackType.Melee:
                PerformMeleeAttack();
                break;

            case EnemyAttackType.Ranged:
                PerformRangedAttack();
                break;
        }

        StartCoroutine(AttackCooldown());
    }
    //攻击行为实现
    private void PerformMeleeAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            playerLayer
        );
        // 新增动画触发
        if (effectRenderer != null)
        {
            PlayAttackEffect();
        }
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerAttributes.Instance.TakeDamage(attackDamage);
                Debug.Log($"近战攻击造成 {attackDamage} 点伤害");
            }
        }
    }
    //武器方向控制
    private void HandleWeaponRotation()
    {
        if (attackType != EnemyAttackType.Ranged || weaponPivot == null) return;

        Vector2 lookDirection = playerTransform.position - weaponPivot.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        weaponPivot.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void PerformRangedAttack()
    {
        if (projectilePrefab == null) return;

        GameObject projectile = Instantiate(
            projectilePrefab,
            weaponPivot.position,
            weaponPivot.rotation
        );

        Vector2 direction = (playerTransform.position - weaponPivot.position).normalized;
        projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;
    }

    private void TransitionToState(AIState newState)
    {
        currentAIState = newState;
        animator.SetBool("IsChasing", newState == AIState.Chasing);
        animator.SetBool("IsAttacking", newState == AIState.Attacking);
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
    private void HandlePlayerDetection()
    {
        // 与状态机联动的检测逻辑
        float distance = Vector3.Distance(transform.position,
            PlayerAttributes.Instance.PlayerTransform.position);

        if (distance < stateMachine.chaseRange)
        {
            EventManager.Instance.TriggerEvent("EnemyStateChanged",
                new EnemyStateEventData(EnemyStateType.Patrol, EnemyStateType.Chase));
        }
    }
    [Header("受击效果")]
    [SerializeField] private ParticleSystem hitEffect;
    // 在EnemyProperty.cs中添加
    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }
    private void Die()
    {// 触发死亡动画
        animator.SetTrigger("Die");

        // 禁用其他组件
        GetComponent<AIPath>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

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
    // 在动画关键帧调用
    public void OnAttackAnimationEvent()
    {
        if (attackType == EnemyAttackType.Melee)
        {
            PerformMeleeAttack();
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
    public void TriggerDeath()
    {
        animator.SetTrigger("Die"); // 触发死亡动画
                                    // 禁用其他组件
        GetComponent<AIPath>().enabled = false;
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