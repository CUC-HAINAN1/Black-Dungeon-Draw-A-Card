using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [Header("弹道参数")]
    public float speed = 15f;
    public int damage = 10;
    public LayerMask obstacleLayer;

    private Vector3 moveDirection;
    private bool isStopped = false;

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    public void Initialize(Vector3 targetPos)
    {
        Debug.Log($"弹道方向计算 | 起始位置: {transform.position} | 目标位置: {targetPos}");

        moveDirection = (targetPos - transform.position).normalized;
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Debug.DrawRay(transform.position, moveDirection * 5f, Color.green, 2f);
    }

    void Update()
    {
        if (!isStopped)
        {
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 添加调试信息
        Debug.Log($"箭矢碰撞对象: {other.name} | 层级: {LayerMask.LayerToName(other.gameObject.layer)}");

        // 忽略友军和触发器
        if (other.CompareTag("Enemy") || other.isTrigger) return;

        // 碰撞障碍物
        if (((1 << other.gameObject.layer) & obstacleLayer) != 0)
        {
            StopArrow();
            return;
        }

        // 击中玩家（根据PlayerAttributes类修改）
        if (other.CompareTag("Player"))
        {
            // 使用正确的单例调用方式
            PlayerAttributes.Instance.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    void StopArrow()
    {
        isStopped = true;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 0.5f);
    }
}