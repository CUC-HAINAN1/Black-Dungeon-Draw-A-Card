using UnityEngine;
using UnityEngine.UI;

public class Projectile : MonoBehaviour
{
    public float speed =10f;
    public int damage = 10;
    void Start ()
    {
        Destroy(this.gameObject,3f);
    }
    void Update ()
    {
        transform.Translate(Vector2.right*speed*Time.deltaTime);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerAttributes.Instance.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}