using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    public abstract void Execute(SkillSystem.ExecutionContext context);

}