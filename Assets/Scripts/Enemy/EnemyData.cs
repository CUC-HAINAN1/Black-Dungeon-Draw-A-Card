using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/EnemyData")]
public class EnemyData : ScriptableObject {
    public int maxHealth;

    public static void CopyEnemyData(EnemyData from, EnemyData to) {

        to.maxHealth = from.maxHealth;

    }

}
