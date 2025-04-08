using UnityEngine;

public class BerserkAnimatorShifter : MonoBehaviour {
    private Animator _animator;
    private int isAttackIncreasedHash = Animator.StringToHash("IsAttackincreased");

    private void OnEnable() {
        
        _animator = gameObject.GetComponent<Animator>();

        EventManager.Instance.Subscribe("AttackIncreasedStateEntered", HandleAttackIncreasedStart);
        EventManager.Instance.Subscribe("AttackIncreasedStateExited", HandleAttackIncreasednd);
    
    }

    private void HandleAttackIncreasedStart(object eventData) {
        
        //Debug.Log("Received AttackIncreasedStateEntered");
        _animator.SetBool(isAttackIncreasedHash, true);

    }

    private void HandleAttackIncreasednd(object eventData) => _animator.SetBool(isAttackIncreasedHash, false);

}