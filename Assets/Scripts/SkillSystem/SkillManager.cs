using UnityEngine;
using System.Collections.Generic;

public class SkillSystem : MonoBehaviour {
    public static SkillSystem Instance { get; private set; }

    private RangeIndicatorManager rangeIndicator;
    private PlayerAttributes playerAttributes;
    private Dictionary<CardDataBase.SkillType, SkillBase> skillHandlers;

    public struct ExecutionContext {
        public CardDataBase cardData;
        public Vector3 direction;
        public Vector3 position;
        public GameObject target;

    }

    private void Awake() {

        Instance = this;

        InitializeSkillHandlers();

    }

    private void InitializeSkillHandlers() {
        skillHandlers = new Dictionary<CardDataBase.SkillType, SkillBase> {
  
            { CardDataBase.SkillType.GenerateShield, gameObject.AddComponent<GenerateShieldSkill>() },
            { CardDataBase.SkillType.InceaseAttck, gameObject.AddComponent<IncreaseAttackPowerSkill>() },
            {CardDataBase.SkillType.Projectile, gameObject.AddComponent<LaunchFireBallSkill>()},
            {CardDataBase.SkillType.Sweep, gameObject.AddComponent<SweepSkill>()},
            {CardDataBase.SkillType.Area, gameObject.AddComponent<PoisonMistSkill>()},
            {CardDataBase.SkillType.BurstAOE, gameObject.AddComponent<MeteoricFireballSkill>()},
            {CardDataBase.SkillType.LockOn, gameObject.AddComponent<LightningSkill>()},

        };
    }

    private ExecutionContext CreateExecutionContext(CardDataBase cardData) {

        rangeIndicator = RangeIndicatorManager.Instance;
        playerAttributes = PlayerAttributes.Instance;

        Vector3 pos = Vector3.zero;
        Vector3 dir = Vector3.zero;
        GameObject target = null;

        switch (cardData.inputMode) {

            case CardDataBase.InputMode.AreaSelection:

                pos = rangeIndicator.GetContext<Vector3>();
                break;

            case CardDataBase.InputMode.DragDirection:

                dir = rangeIndicator.GetContext<Vector3>();
                break;

            case CardDataBase.InputMode.TargetLock:

                target = rangeIndicator.GetContext<GameObject>();
                break;

        }

        return new ExecutionContext {

            cardData = cardData,
            direction = dir,
            position = pos,
            target = target,

        };
    }

    public void ExecuteSkill(CardDataBase cardData) {

        if (!skillHandlers.TryGetValue(cardData.skillType, out var handler)) {

            CustomLogger.LogWarning($"No handler found for skill type: {cardData.skillType}");
            return;

        }

        var context = CreateExecutionContext(cardData);
        handler.Execute(context);

    }
}
