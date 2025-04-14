using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [Header("��������")]
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
        Debug.Log($"����������� | ��ʼλ��: {transform.position} | Ŀ��λ��: {targetPos}");

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
        // ���ӵ�����Ϣ
        Debug.Log($"��ʸ��ײ����: {other.name} | �㼶: {LayerMask.LayerToName(other.gameObject.layer)}");

        // �����Ѿ��ʹ�����
        if (other.CompareTag("Enemy") || other.isTrigger) return;
        
        // ������ң�����PlayerAttributes���޸ģ�
        else if (other.CompareTag("Player"))
        {

            // ʹ����ȷ�ĵ������÷�ʽ
            PlayerAttributes.Instance.TakeDamage(damage);
            Destroy(gameObject);
        }

        // ��ײ�ϰ���
        else if (((1 << other.gameObject.layer) & obstacleLayer) != 0) {
            StopArrow();
            return;
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
