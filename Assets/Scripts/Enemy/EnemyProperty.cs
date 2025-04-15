using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine.Rendering;

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

    [Header("追击设置")]
    [SerializeField] private float chaseSpeed = 6f;          // 新增：独立追击速度
    [SerializeField] private float chaseRotationSpeed = 360f; // 新增：追击转向速度
    [SerializeField][Range(0.1f, 1f)] private float pathRefreshRate = 0.5f; // 新增：路径刷新频率
    [SerializeField] private float predictionFactor = 0.3f;   // 新增：玩家移动预测系数
    [SerializeField] private float chaseStoppingDistance = 0.5f; // 新增：追击停止距离


    [Header("远程攻击配置")]
    public GameObject arrowPrefab;
    public Transform shootPoint; // 箭矢生成点
    public LayerMask obstacleLayers;

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

    // 新增速度曲线控制（在类中添加）
    [Header("移动曲线")]
    [SerializeField]
    private AnimationCurve speedCurve = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(1, 1)
    );
    private void UpdateMovement()
    {
        var aiPath = GetComponent<AIPath>();
        if (aiPath == null) return;


        // 强制重置路径
        if (aiPath.isStopped)
        {
            aiPath.canMove = true;
            aiPath.SearchPath();
        }

        // 根据距离动态调整速度
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        float speedFactor = speedCurve.Evaluate(distanceToPlayer / detectionRange);

        aiPath.maxSpeed = patrolSpeed * (currentAIState == AIState.Chasing ? 2f : 1f) * speedFactor;
        // 添加调试信息
        Debug.Log($"移动状态: {aiPath.canMove} | 速度: {aiPath.maxSpeed} | 目标: {aiPath.destination}");
    }

    void Awake()
    {
        currentHealth = maxHealth;
        healthUI = GetComponentInChildren<EnemyHealthUI>();
        healthUI?.Initialize(maxHealth);
    }

    void Start()
    { // 确保AIPath组件存在
        if (GetComponent<AIPath>() == null)
        {
            gameObject.AddComponent<AIPath>();
            Debug.Log("已自动添加AIPath组件");
        }
        // 延迟初始化玩家坐标
        StartCoroutine(DelayedInit());
        GeneratePatrolPoints(); // 新增此行
        // 确保playerTransform正确初始化
        if (PlayerAttributes.Instance == null)
        {
            Debug.LogError("PlayerAttributes实例未找到!");
            return;
        }
        playerTransform = PlayerAttributes.Instance.PlayerTransform;

        // 初始化特效系统
        InitializeEffectSystem();

        // 初始化动画组件
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
                Debug.LogError("Animator组件未找到!", gameObject);
        }
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
            //effectRenderer = effectContainer.AddComponent<SpriteRenderer>();
            effectRenderer.enabled = false;
        }
    }
    private IEnumerator DelayedInit()
    {
        yield return new WaitUntil(() => PlayerAttributes.Instance != null);
        playerTransform = PlayerAttributes.Instance.PlayerTransform;

        // 强制初始状态
        TransitionToState(AIState.Patrolling);
    }
    // 新增特效初始化方法
    private void InitializeEffectSystem()
    {
        if (effectContainer == null)
        {
            effectContainer = new GameObject("AttackEffect");
            effectContainer.transform.SetParent(transform);
            effectContainer.transform.localPosition = Vector3.zero;
        }

        effectRenderer = effectContainer.GetComponent<SpriteRenderer>();
        if (effectRenderer == null)
        {
            effectRenderer = effectContainer.AddComponent<SpriteRenderer>();
            effectRenderer.sortingOrder = 10;
        }
        effectRenderer.enabled = false;
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
        if (!IsAlive() || playerTransform == null) return; // 原条件写反了

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
        // 动画混合控制
        HandleAnimationBlending();

        UpdateMovement(); // 新增此行
    }

    private void HandleAnimationBlending()
    {
        // 攻击状态时禁用其他混合逻辑
        if (currentAIState == AIState.Attacking) return;
        // 根据移动速度调整动画混合
        var aiPath = GetComponent<AIPath>();
        if (aiPath != null)
        {
            float speedRatio = aiPath.velocity.magnitude / aiPath.maxSpeed;
            animator.SetFloat("MoveSpeed", speedRatio);
        }

        // 攻击动画强制播放
        if (currentAIState == AIState.Attacking)
        {
            animator.Play("Attack", 0, 0);
        }
    }
    // 在动画最后一帧添加事件
    public void OnAttackEnd()
    {
        TransitionToState(AIState.Chasing);
    }
    //追击行为实现
    private void ChaseBehavior()
    {

        var aiPath = GetComponent<AIPath>();
        // 强制保持移动能力
        aiPath.canMove = true;
        // 强制刷新路径（原逻辑有缺陷）
        Vector3 predictedPos = PlayerAttributes.Instance.PlayerTransform.position;
        if (PlayerAttributes.Instance.TryGetComponent<Rigidbody2D>(out var rb))
        {
            predictedPos += (Vector3)(rb.velocity * 0.5f); // 增加预测系数
        }

        if (aiPath == null) return;
        // 设置绝对追击速度（不受其他系统影响）
        float targetSpeed = patrolSpeed * 2f;
        // 动态计算速度系数
        float chaseSpeedMultiplier = 2f;

    if (Vector3.Distance(aiPath.destination, playerTransform.position) > 0.5f)
        {
            aiPath.destination = playerTransform.position;
            aiPath.SearchPath(); // 强制刷新路径
        }
        // 移除速度曲线干扰
        aiPath.maxSpeed = chaseSpeed; // 修改：使用独立速度
        aiPath.rotationSpeed = chaseRotationSpeed; // 新增：应用转向速度
        aiPath.endReachedDistance = chaseStoppingDistance; // 修改：使用配置的停止距离
        aiPath.slowdownDistance = attackRange * 0.8f;     // 开始减速距离
         // 按频率刷新路径（优化性能）
    if (Time.frameCount % Mathf.RoundToInt(pathRefreshRate * 60) == 0) // 新增：按频率刷新
    {
        // 预测玩家移动轨迹
        Rigidbody2D playerRb = playerTransform.GetComponent<Rigidbody2D>();
        Vector3 predictPos = playerTransform.position +
                            (Vector3)(playerRb != null ?
                             playerRb.velocity * predictionFactor : // 使用预测系数
                             Vector2.zero);
         aiPath.destination = predictPos;

        if (Vector3.Distance(aiPath.destination, predictPos) > 0.5f)
        {
            aiPath.destination = predictPos;
            aiPath.SearchPath();
        }
    }
        // 强制刷新路径
        if (!aiPath.hasPath)
        {
            aiPath.destination = playerTransform.position;
            aiPath.SearchPath();
        }
        // 设置加速参数（需在Animator中添加SpeedMultiplier参数）
        animator.SetFloat("SpeedMultiplier", chaseSpeedMultiplier);

        aiPath.destination = playerTransform.position;

        // 调试日志
        Debug.Log($"进入追击状态，当前速度: {aiPath.maxSpeed}", gameObject);

    }
    // 新增巡逻点生成方法（在EnemyProperty类中添加）
    private void GeneratePatrolPoints()
    {
        // 如果已有预设点则跳过
        if (patrolPoints != null && patrolPoints.Length > 0) return;

        // 方式1：查找场景中带Waypoint标签的对象
        var waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        if (waypoints.Length >= 2)
        {
            patrolPoints = new Transform[waypoints.Length];
            for (int i = 0; i < waypoints.Length; i++)
            {
                patrolPoints[i] = waypoints[i].transform;
            }
            Debug.Log($"从场景中找到{waypoints.Length}个巡逻点");
            return;
        }

        if (patrolPoints.Length == 0)
        {
            // 自动查找场景中的巡逻点
            patrolPoints = new Transform[waypoints.Length];
            for (int i = 0; i < waypoints.Length; i++)
            {
                patrolPoints[i] = waypoints[i].transform;
            }

            // 或生成随机路径
            if (waypoints.Length == 0)
            {
                GenerateRandomPath(3, 5f); // 生成3个点，范围5米
            }
        }
    }

    // 随机路径生成算法
    private void GenerateRandomPath(int pointCount, float radius)
    {
        patrolPoints = new Transform[pointCount];
        for (int i = 0; i < pointCount; i++)
        {
            GameObject point = new GameObject($"PatrolPoint_{i}");
            point.transform.position = transform.position + new Vector3(
                Random.Range(-radius, radius),
                Random.Range(-radius, radius),
                0
            );
            patrolPoints[i] = point.transform;
        }
    }


    //巡逻系统优化
    private void PatrolBehavior()
    {
        if (currentAIState != AIState.Patrolling) return; // 状态保护
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
        if (playerTransform == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        // 添加空引用保护
        if (playerTransform == null)
        {
            Debug.LogWarning("玩家Transform未初始化!");
            return;
        }
        // 调试可视化
        Debug.DrawLine(transform.position, playerTransform.position,
                     currentAIState == AIState.Attacking ? Color.red : Color.yellow);

        switch (currentAIState)
        {
            case AIState.Patrolling:
                // 明确追击条件：必须在检测范围内且不在冷却中
                if (distanceToPlayer <= detectionRange)
                {
                    TransitionToState(AIState.Chasing);
                    StopAllCoroutines(); // 确保停止所有协程
                    isPatrolling = false;
                    GetComponent<AIPath>().destination = playerTransform.position; // 立即更新目标
                }
                break;

            case AIState.Chasing:
                // 双重退出条件：超出追出范围或有攻击机会
                if (distanceToPlayer > detectionRange * 1.5f)
                {
                    TransitionToState(AIState.Patrolling);
                    StartPatrol(); // 重新开始巡逻
                }
                else if (distanceToPlayer <= attackRange && canAttack)
                {
                    TransitionToState(AIState.Attacking);
                }
                break;

            case AIState.Attacking:
                // 增加双重退出条件
                bool animEnded = animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f;
                if (distanceToPlayer > attackRange * 1.2f || animEnded)
                {
                    TransitionToState(AIState.Chasing);
                }
                break;
        }

        // 添加调试信息
        Debug.Log($"当前状态: {currentAIState} | 玩家距离: {distanceToPlayer} | 检测范围: {detectionRange}");

        // 添加状态切换保护
        if (currentAIState == AIState.Attacking && !canAttack) return;
        switch (currentAIState)
        {
            case AIState.Chasing:
                ChaseBehavior();
                break;
            case AIState.Patrolling:
                PatrolBehavior();
                break;
            case AIState.Attacking:
                // 攻击行为已通过动画事件处理
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
        // 强制重置动画状态
        animator.Play("Attack", 0, 0f);
        animator.Update(0); // 立即更新
                            // 扩展有效攻击范围（三维检测）
        Vector3 detectSize = new Vector3(attackRadius * 2, attackRadius * 2, 1f);
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            attackPoint.position,
            detectSize,
            0f,
            playerLayer);
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
        // 添加动画事件监听
        StartCoroutine(CheckAttackAnimationProgress());
        // 添加攻击延迟补偿（解决动画同步问题）
        StartCoroutine(DelayedDamage(hits, 0.2f)); // 延迟0.2秒生效
    }

    private IEnumerator DelayedDamage(Collider2D[] hits, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var hit in hits)
        {
            if (hit == null)
                continue;

            if (hit.CompareTag("Player")) {
                PlayerAttributes.Instance.TakeDamage(attackDamage);
                Debug.Log($"延迟伤害生效: {attackDamage}");
            }
        }
    }
    private IEnumerator CheckAttackAnimationProgress()
    {
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f)
        {
            yield return null;
        }
        TransitionToState(AIState.Chasing);
    }

    // 新增动画结束回调
    private IEnumerator ResetAttackState(float delay)
    {
        yield return new WaitForSeconds(delay * 0.9f); // 提前10%退出状态

        if (currentAIState == AIState.Attacking)
        {
            TransitionToState(AIState.Chasing);
            Debug.Log("攻击动画结束，返回追击状态");
        }
    }
    //武器方向控制
    private void HandleWeaponRotation()
    {
        if (attackType != EnemyAttackType.Ranged || weaponPivot == null || playerTransform == null) return;

        Vector2 lookDirection = playerTransform.position - weaponPivot.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        weaponPivot.rotation = Quaternion.Euler(0, 0, angle);
    }

#if UNITY_EDITOR
    void OnValidate()
    {

        // 自动查找武器节点
        if (shootPoint == null)
        {
            Transform weapon = transform.Find("WeaponPivot");
            if (weapon != null) shootPoint = weapon;
        }
    }
#endif
    public void PerformRangedAttack()
    {
        // 输入验证
        if (arrowPrefab == null)
        {
            Debug.LogError("箭矢预制体未分配!", gameObject);
            return;
        }
        if (shootPoint == null)
        {
            Debug.LogError("攻击点未配置!", gameObject);
            return;
        }
        if (PlayerAttributes.Instance == null)
        {
            Debug.LogError("玩家属性实例未找到!", gameObject);
            return;
        }

        // 计算预测位置（带提前量）
        Vector3 playerVelocity = Vector3.zero;
        Rigidbody2D playerRb = PlayerAttributes.Instance.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerVelocity = playerRb.velocity;
        }
        Vector3 predictedPosition = PlayerAttributes.Instance.PlayerTransform.position +
                                   playerVelocity * 0.3f;

        // 计算发射方向
        Vector2 shootDirection = (predictedPosition - shootPoint.position).normalized;

        // 生成箭矢
        GameObject arrow = Instantiate(
            arrowPrefab,
            shootPoint.position,
            Quaternion.identity
        );

        // 获取弹道组件
        ArrowProjectile projectile = arrow.GetComponent<ArrowProjectile>();
        if (projectile == null)
        {
            Debug.LogError("箭矢预制体缺少 ArrowProjectile 组件!", arrow);
            Destroy(arrow);
            return;
        }

        // 初始化弹道
        try
        {
            projectile.Initialize(predictedPosition);
            projectile.obstacleLayer = obstacleLayers;

            // 调试可视化
            Debug.DrawLine(shootPoint.position, predictedPosition, Color.red, 1f);
            Debug.Log($"箭矢已生成 | 位置: {shootPoint.position} | 目标: {predictedPosition}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"弹道初始化失败: {e.Message}", arrow);
            Destroy(arrow);
        }

        // 触发攻击冷却
        StartCoroutine(AttackCooldownRoutine());
    }

    private IEnumerator AttackCooldownRoutine()
    {
        canAttack = false;
        Debug.Log($"攻击冷却开始 | 持续时间: {attackInterval}秒");
        // 添加冷却计时可视化

        float timer = 0;
        while (timer < attackInterval)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(attackInterval);
        canAttack = true;
        Debug.Log("攻击冷却结束");
    }

    private void TransitionToState(AIState newState)
    {
        // 在状态切换时应用参数
        var aiPath = GetComponent<AIPath>();
        // 需要增加动画触发器重置
        animator.ResetTrigger("Chase");
        animator.ResetTrigger("Attack");
        // 状态重复检查
        if (currentAIState == newState) return;
        // 退出当前状态处理
        switch (currentAIState)
        {
            case AIState.Attacking:
                animator.ResetTrigger("Attack");
                break;
        }
        // 进入新状态处理
        switch (newState)
        {
            case AIState.Attacking:
                animator.SetTrigger("AttackTrigger");
                GetComponent<AIPath>().canMove = false; // 攻击时停止移动
                break;
            case AIState.Chasing:
                aiPath.slowdownDistance = attackRange * 0.8f; // 修改：关联攻击范围
                aiPath.rotationSpeed = chaseRotationSpeed;    // 新增：确保转向速度生效
                GetComponent<AIPath>().canMove = true;
                break;
        }

        Debug.Log($"状态切换: {currentAIState} -> {newState}");
        currentAIState = newState;
        // 应改为使用Trigger驱动：
        switch (newState)
        {
            case AIState.Chasing:
                animator.ResetTrigger("Attack");
                animator.SetTrigger("Chase");
                break;
            case AIState.Attacking:
                animator.ResetTrigger("Chase");
                animator.SetTrigger("Attack");
                break;
        }
        // 立即应用状态变化
        animator.Update(0);
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
        if (currentAIState != AIState.Patrolling) return; // 状态保护
        // 确保至少有两个巡逻点才会开始巡逻
        if (patrolPoints.Length < 2)
        {
            Debug.LogWarning("巡逻点不足，无法开始巡逻");
            return;
        }
        if (patrolPoints.Length < 2) return;
        currentPatrolIndex = 0;
        isPatrolling = true;

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
