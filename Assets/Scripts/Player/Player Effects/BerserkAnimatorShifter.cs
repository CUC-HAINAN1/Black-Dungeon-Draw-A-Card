using UnityEngine;

public class BerserkAnimatorShifter : MonoBehaviour {
    public Animator animator;
    private int isAttackIncreasedHash = Animator.StringToHash("IsAttackincreased");

    private void OnEnable() {

        EventManager.Instance.Subscribe("AttackIncreasedStateEntered", HandleAttackIncreasedStart);
        EventManager.Instance.Subscribe("AttackIncreasedStateExited", HandleAttackIncreasedEnd);

    }

    private void OnDisable() {

        if (EventManager.Instance != null) {

            EventManager.Instance.Unsubscribe("AttackIncreasedStateEntered", HandleAttackIncreasedStart);
            EventManager.Instance.Unsubscribe("AttackIncreasedStateExited", HandleAttackIncreasedEnd);

        }
    }

    private void HandleAttackIncreasedStart(object eventData) {

        //Debug.Log("Received AttackIncreasedStateEntered");
        animator.SetBool(isAttackIncreasedHash, true);

    }

    private void HandleAttackIncreasedEnd(object eventData) => animator.SetBool(isAttackIncreasedHash, false);

}
