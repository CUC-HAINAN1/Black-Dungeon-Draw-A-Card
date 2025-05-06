using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "Game/BossData")]
public class BossData : ScriptableObject {
    public float moveSpeed;
    public float skillCooldown;
    public int maxHealth;

    public static void CopyBossData(BossData from, BossData to) {

        to.moveSpeed = from.moveSpeed;
        to.skillCooldown = from.skillCooldown;
        to.maxHealth = from.maxHealth;

    }


}

