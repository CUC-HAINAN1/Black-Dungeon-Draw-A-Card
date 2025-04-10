using UnityEngine;
using System.Collections;

public class IncreaseAttackPowerSkill : SkillBase {
    public override void Execute(SkillSystem.ExecutionContext context) {

        var atteckPowerConfig = context.cardData.behaviorConfig.inceaseAttck;
        int increaseAmount = atteckPowerConfig.amount;
        float duration = atteckPowerConfig.duration;

        StartCoroutine(IncreaseAttackRoutine(increaseAmount, duration));
    
    }

    private IEnumerator IncreaseAttackRoutine(int amount, float duration) {
        
        PlayerAttributes playerAttributes = PlayerAttributes.Instance;

        playerAttributes.IncreaseAttackPower(amount);

        // 等待护盾持续时间
        yield return new WaitForSeconds(duration);
            
        playerAttributes.DecreaseAttackPower(amount);

    }
}