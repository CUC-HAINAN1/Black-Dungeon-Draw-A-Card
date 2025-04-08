using UnityEngine;
using System.Collections;

public class GenerateShieldSkill : SkillBase {
    public override void Execute(SkillSystem.ExecutionContext context) {

        var shieldConfig = context.cardData.behaviorConfig.generateShield;
        int shieldAmount = shieldConfig.amount;
        float duration = shieldConfig.duration;

        StartCoroutine(ShieldRoutine(shieldAmount, duration));
    
    }

    private IEnumerator ShieldRoutine(int amount, float duration) {
        
        PlayerAttributes playerAttributes = PlayerAttributes.Instance;

        playerAttributes.GenerateShield();
        
        // 等待护盾持续时间
        yield return new WaitForSeconds(duration);
            
        playerAttributes.VanishShield();
    
    }
}