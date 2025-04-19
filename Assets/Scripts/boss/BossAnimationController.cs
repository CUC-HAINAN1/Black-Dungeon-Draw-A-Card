using UnityEngine;

public class BossAnimationController : MonoBehaviour
{
    private Animator animator;
    private BossAI bossAI;

    // 动画参数定义
    private static readonly int AttackType = Animator.StringToHash("AttackType");
    private static readonly int AOEStep = Animator.StringToHash("AOEStep");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    
    void Start()
    {
        animator = GetComponent<Animator>();
        bossAI = GetComponent<BossAI>();

        // 注册AOE阶段切换事件
        bossAI.OnAOEPhaseChanged += HandleAOEPhase;
    }

    void Update()
    {
        // 基础状态控制
        if (!bossAI.IsSkillActive)
        {
            animator.SetInteger(AttackType, 0); // 返回待机状态
        }
    }

    // 处理不同攻击类型的动画
    public void PlayAttackAnimation(int type)
    {
        animator.SetInteger(AttackType, type);
        animator.SetBool(IsAttacking, true);

        // 根据类型设置持续时间
        switch (type)
        {
            case 1: // 冲刺
                StartCoroutine(ResetAfter(0.5f));
                break;
            case 2: // 弹幕
                StartCoroutine(ResetAfter(0.2f));
                break;
        }
    }

    // AOE阶段处理（0:准备 1:上升 2:下砸）
    private void HandleAOEPhase(int phase)
    {
        animator.SetInteger(AOEStep, phase);
        animator.SetBool(IsAttacking, true);

        if (phase == 2) // 下砸完成后重置
        {
            StartCoroutine(ResetAfter(1.0f));
        }
    }

    private System.Collections.IEnumerator ResetAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        animator.SetBool(IsAttacking, false);
        animator.SetInteger(AttackType, 0);
    }
}
