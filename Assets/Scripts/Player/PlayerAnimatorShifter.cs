using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
public class PlayerAnimatorShifter : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Animator shieldAnimator;
    
    // 使用哈希存储动画参数
    private int isMovingHash = Animator.StringToHash("IsMoving");
    private int deathHash = Animator.StringToHash("IsDead");
    private int invincibleHash = Animator.StringToHash("IsInvincible");
    private int rollingHash = Animator.StringToHash("IsRolling");        
    
    private void OnEnable() {
    
        SubscribeEvents();
        movement.OnMovementInputChanged += HandleMovement;
    
    }

    //private void OnDisable() {
    
        //UnsubscribeEvents();
        //movement.OnMovementInputChanged -= HandleMovement;
    
    //}

    private void SubscribeEvents() {
        
        //死亡事件
        EventManager.Instance.Subscribe("PlayerDied", HandleDeath);
        
        //无敌状态进入与退出事件
        EventManager.Instance.Subscribe("InvincibleStateEntered", HandleInvincibleStart);
        EventManager.Instance.Subscribe("InvincibleStateExited", HandleInvincibleEnd);

        //翻滚进入与退出事件
        EventManager.Instance.Subscribe("RollingStateEntered", HandleRollingStart);
        EventManager.Instance.Subscribe("RollingStateExited", HandleRollingEnd);

    }

    private void UnsubscribeEvents() {
    
        //死亡事件
        EventManager.Instance.Unsubscribe("PlayerDied", HandleDeath);
        
        //无敌状态进入与退出事件
        EventManager.Instance.Unsubscribe("InvincibleStateEntered", HandleInvincibleStart);
        EventManager.Instance.Unsubscribe("InvincibleStateExited", HandleInvincibleEnd);
        
        //翻滚进入与退出事件
        EventManager.Instance.Unsubscribe("RollingStateEntered", HandleRollingStart);
        EventManager.Instance.Unsubscribe("RollingStateExited", HandleRollingEnd);
    
    }

    private void HandleMovement(Vector2 input) => animator.SetBool(isMovingHash, input.magnitude > 0.1f);

    private void HandleDeath(object eventData) => animator.SetBool(deathHash, true);

    private void HandleInvincibleStart(object eventData) => animator.SetBool(invincibleHash, true);
    private void HandleInvincibleEnd(object eventData) => animator.SetBool(invincibleHash, false);

    private void HandleRollingStart(object eventData) => animator.SetBool(rollingHash, true);
    private void HandleRollingEnd(object eventData) => animator.SetBool(rollingHash, false);


}
