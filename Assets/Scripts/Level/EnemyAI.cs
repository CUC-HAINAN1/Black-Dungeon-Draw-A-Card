using UnityEngine;
using Pathfinding;
using System.Collections;

[RequireComponent(typeof(Seeker), typeof(AIPath), typeof(AIDestinationSetter))]
public class EnemyAI : MonoBehaviour
{
    [Header("PatrolSet")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float switchWaitTime = 1f;

    [Header("ChaseSet")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float stopChaseRange = 7f;

    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;
    private int currentPatrolIndex;
    private bool isChasing;

    void Awake()
    {
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath.maxSpeed = patrolSpeed;
    }

    void Start()
    {
        StartCoroutine(PatrolRoutine());
    }

    void Update()
    {
        CheckPlayerDistance();
        UpdateAnimation(); // ���ö�������
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            if (patrolPoints.Length > 0)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                destinationSetter.target = patrolPoints[currentPatrolIndex];

                // �ȴ�����Ŀ���
                yield return new WaitUntil(() => aiPath.reachedEndOfPath);
                yield return new WaitForSeconds(switchWaitTime);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void CheckPlayerDistance()
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance < chaseRange && !isChasing)
        {
            StartChasing(player.transform);
        }
        else if (distance > stopChaseRange && isChasing)
        {
            StopChasing();
        }
    }

    private void StartChasing(Transform target)
    {
        isChasing = true;
        StopAllCoroutines();
        aiPath.maxSpeed = chaseSpeed;
        destinationSetter.target = target;
    }

    private void StopChasing()
    {
        isChasing = false;
        aiPath.maxSpeed = patrolSpeed;
        StartCoroutine(PatrolRoutine());
        destinationSetter.target = patrolPoints[currentPatrolIndex];
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopChaseRange);
    }
    [Header("��������")]
    [SerializeField] private Animator animator;
    [SerializeField] private string patrolAnim = "Walk";
    [SerializeField] private string chaseAnim = "Run";
    [SerializeField] private string idleAnim = "Idle";

    private void UpdateAnimation()
    {
        if (aiPath.reachedEndOfPath)
        {
            animator.Play(idleAnim);
        }
        else
        {
            animator.Play(isChasing ? chaseAnim : patrolAnim);
        }
    }

}
