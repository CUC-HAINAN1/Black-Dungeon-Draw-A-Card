using UnityEditor.Animations;
using UnityEngine;

public class ShieldAnimatorShifter : MonoBehaviour {
    public Animator animator;
    private int shieldHash = Animator.StringToHash("IsShield");

    private void OnEnable() {

        EventManager.Instance.Subscribe("ShieldStateEntered", HandleShieldStart);
        EventManager.Instance.Subscribe("ShieldStateExited", HandleShieldEnd);

    }

    private void OnDisable() {

        if (EventManager.Instance != null) {

            EventManager.Instance?.Unsubscribe("ShieldStateEntered", HandleShieldStart);
            EventManager.Instance?.Unsubscribe("ShieldStateExited", HandleShieldEnd);

        }
    }

    private void HandleShieldStart(object eventData) => animator.SetBool(shieldHash, true);
    private void HandleShieldEnd(object eventData) => animator.SetBool(shieldHash, false);


}
