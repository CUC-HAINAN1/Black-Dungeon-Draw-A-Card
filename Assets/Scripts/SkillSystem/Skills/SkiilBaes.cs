using UnityEngine;
public abstract class SkillBase : MonoBehaviour
{
    public struct SkillParams
    {
        public Vector2 direction;
        public Vector3 targetPosition;
        public CardDataBase.SkillBehaviorConfig config;
        public GameObject lockedTargets;
        public Vector3 casterPosition;
    }

    public abstract void Execute(SkillParams parameters);

}