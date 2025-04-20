using UnityEngine;

[CreateAssetMenu(fileName = "BossStats", menuName = "Game/BossStats")]
public class BossData : ScriptableObject {
    public float moveSpeed;
    public float skillCooldown;
    public int maxHealth;

}
