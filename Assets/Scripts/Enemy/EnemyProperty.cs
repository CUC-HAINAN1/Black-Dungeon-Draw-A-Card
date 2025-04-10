using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Pathfinding;

public class EnemyProperty : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2f;

    [Header("Ѳ������")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waitDuration = 1f;

    [Header("��������")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private float destroyDelay = 2f;

    // �¼�
    public UnityEvent<EnemyProperty> OnDeath;
    public UnityEvent<int> OnHealthChanged;

    private int currentHealth;
    private float lastAttackTime;
    private int currentPatrolIndex;
    private bool isPatrolling;

    // Ѫ��UI���
    private EnemyHealthUI healthUI;

    [Header("��������")]
    [SerializeField] private Animator animator;

    [Header("״̬����")]
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
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }
    void Update()
    { // �����ƶ�״̬���²���
        if (IsAlive())
        {
            // �����ƶ�״̬���²���
            bool isMoving = GetComponent<AIPath>().remainingDistance > 0.1f;
            animator.SetBool("IsMoving", isMoving);
        }
        if (isPatrolling)
        {
            PatrolMovement();
        }
    }

    public void TakeDamage(int damage)
    {
        if (!IsAlive()) return; // ��ֹ�ظ��˺�

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
        // ��״̬�������ļ���߼�
        float distance = Vector3.Distance(transform.position,
            PlayerAttributes.Instance.PlayerTransform.position);

        if (distance < stateMachine.chaseRange)
        {
            EventManager.Instance.TriggerEvent("EnemyStateChanged",
                new EnemyStateEventData(EnemyStateType.Patrol, EnemyStateType.Chase));
        }
    }
    [Header("�ܻ�Ч��")]
    [SerializeField] private ParticleSystem hitEffect;
    // ��EnemyProperty.cs�����
    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }
    private void Die()
    {// ������������
        animator.SetTrigger("Die");

        // �����������
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
    public void KillInstantly()
    {
        // ֱ����ֹ��������
        currentHealth = 0;
        StopAllCoroutines();
        isPatrolling = false;

        // ����������������
        healthUI?.UpdateHealth(0);
        Die();

        // �������ض���
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
        animator.SetTrigger("Die"); // ������������
                                    // �����������
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
