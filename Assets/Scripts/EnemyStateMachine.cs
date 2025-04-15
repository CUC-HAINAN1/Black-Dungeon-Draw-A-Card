using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker), typeof(AIPath))]
public class EnemyStateMachine : MonoBehaviour
{
    [Header("状态参数")]
    [Header("Detection Ranges")]
    [SerializeField] public float chaseRange = 5f;
    [SerializeField] private float attackRange = 2f;
    // 通过属性封装私有字段（推荐方式）
    public float AttackRange => attackRange;
    
    [Header("Movement Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;

    [Header("引用")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;

    // 动画参数哈希
    private int isMovingHash = Animator.StringToHash("IsMoving");
    private int isChasingHash = Animator.StringToHash("IsChasing");
    private int attackHash = Animator.StringToHash("Attack");
    private int deathHash = Animator.StringToHash("Die");

    private AIPath aiPath;
    private int currentPatrolIndex;
    private bool isDead;
    // 在初始化时注册事件
    void Start()
    {
        EventManager.Instance.Subscribe("EnemyStateChanged", HandleStateChange);
    }
    private void HandleStateChange(object eventData)
    {
        var data = (EnemyStateEventData)eventData;
        Debug.Log($"State Changed: {data.PreviousState} → {data.NewState}");

        switch (data.NewState)
        {
            case EnemyStateType.Chase:
                aiPath.maxSpeed = chaseSpeed;
                break;
            case EnemyStateType.Patrol:
                aiPath.maxSpeed = patrolSpeed;
                break;
        }
    }
    void Awake()
    {
        aiPath = GetComponent<AIPath>();
        aiPath.maxSpeed = patrolSpeed;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 状态切换逻辑
        if (distanceToPlayer <= chaseRange)
        {
            EnterChaseState();
            if (distanceToPlayer <= attackRange) TriggerAttack();
        }
        else
        {
            EnterPatrolState();
        }

        UpdateAnimations();
    }

    private void EnterPatrolState()
    {
        aiPath.maxSpeed = patrolSpeed;
        PatrolMovement();
        animator.SetBool(isChasingHash, false);
    }

    private void EnterChaseState()
    {
        aiPath.maxSpeed = chaseSpeed;
        aiPath.destination = player.position;
        animator.SetBool(isChasingHash, true);
    }

    private void PatrolMovement()
    {
        if (patrolPoints.Length == 0) return;

        if (aiPath.reachedEndOfPath)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            aiPath.destination = patrolPoints[currentPatrolIndex].position;
        }
    }

    private void UpdateAnimations()
    {
        animator.SetBool(isMovingHash, aiPath.remainingDistance > 0.1f);
    }

    public void TriggerAttack()
    {
        animator.SetTrigger(attackHash);
        // 实际攻击逻辑
    }

    public void TriggerDeath()
    {
        isDead = true;
        animator.SetTrigger(deathHash);
        aiPath.enabled = false;
        GetComponent<Collider>().enabled = false;
    }
}
