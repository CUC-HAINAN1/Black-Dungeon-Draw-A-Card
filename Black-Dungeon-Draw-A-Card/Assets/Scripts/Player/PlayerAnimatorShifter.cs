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
    private int _isMovingHash = Animator.StringToHash("IsMoving");
    private int _deathHash = Animator.StringToHash("IsDead");
    private int _ShieldHash = Animator.StringToHash("IsShield");
    private int _InvincibleHash = Animator.StringToHash("IsInvincible");        
    
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
        
        //护盾状态进入与退出事件
        EventManager.Instance.Subscribe("ShieldStateEntered", HandleShieldStart);
        EventManager.Instance.Subscribe("ShieldStateExited", HandleShieldEnd);
    
    }

    private void UnsubscribeEvents() {
    
        //死亡事件
        EventManager.Instance.Unsubscribe("PlayerDied", HandleDeath);
        
        //无敌状态进入与退出事件
        EventManager.Instance.Unsubscribe("InvincibleStateEntered", HandleInvincibleStart);
        EventManager.Instance.Unsubscribe("InvincibleStateExited", HandleInvincibleEnd);
        
        //护盾状态进入与退出事件
        EventManager.Instance.Unsubscribe("ShieldStateEntered", HandleShieldStart);
        EventManager.Instance.Unsubscribe("ShieldStateExited", HandleShieldEnd);
    
    }

    private void HandleMovement(Vector2 input) {

        animator.SetBool(_isMovingHash, input.magnitude > 0.1f);

    }

    private void HandleDeath(object eventData) {
    
        animator.SetBool(_deathHash, true);
    
    }

    private void HandleShieldStart(object eventData) => shieldAnimator.SetBool(_ShieldHash, true);
    private void HandleShieldEnd(object eventData) => shieldAnimator.SetBool(_ShieldHash, false);

    private void HandleInvincibleStart(object eventData) => animator.SetBool(_InvincibleHash, true);
    private void HandleInvincibleEnd(object eventData) => animator.SetBool(_InvincibleHash, false);

}
