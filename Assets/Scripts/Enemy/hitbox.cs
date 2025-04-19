using UnityEngine;

public class HitBox : MonoBehaviour {

    [SerializeField] private int damage = 10;

    private void OnTriggerEnter2D(Collider2D other) {

        if (other.CompareTag("Player")) {

            // 造成伤害
            PlayerAttributes.Instance.TakeDamage(damage);
            Debug.Log("Dash hit player!");

        }
    }
}
