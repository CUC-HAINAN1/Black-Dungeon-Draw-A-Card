using UnityEngine;
using System.Collections.Generic;
public class SkillSystem : MonoBehaviour {
    private struct ExecutionContext {
        public CardDataBase cardData;
        public Vector3 direction;
        public Vector3 position;
        public GameObject lockedTargets;

    }

    public SkillSystem Instance { get; private set; }

    private RangeIndicatorManager rangeIndicator;
    private PlayerAttributes player;
    private Dictionary<CardDataBase.SkillType, SkillBase> skillHandlers;

    private void Awake() {

        Instance = this;
        rangeIndicator = RangeIndicatorManager.Instance;
        player = PlayerAttributes.Instance;

    }

    private ExecutionContext CreateExecutionContext(CardDataBase cardData) {
        
        return new ExecutionContext {
            
            cardData = cardData,
            position = (cardData.inputMode == CardDataBase.InputMode.AreaSelection) ?
                rangeIndicator.GetContext<Vector3>() : Vector3.zero,

            direction = (cardData.inputMode == CardDataBase.InputMode.DragDirection) ?
                rangeIndicator.GetContext<Vector3>() : Vector3.zero,
            
            lockedTargets = (cardData.inputMode == CardDataBase.InputMode.TargetLock) ?
                null : null,

        };
    
    }
    













}