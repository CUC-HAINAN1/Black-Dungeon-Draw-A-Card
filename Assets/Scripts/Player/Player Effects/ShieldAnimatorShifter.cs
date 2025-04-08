using UnityEditor.Animations;
using UnityEngine;

public class ShieldAnimatorShifter : MonoBehaviour {
    private Animator _animator;
    private int shieldHash = Animator.StringToHash("IsShield");

    private void OnEnable() {
        
        _animator = gameObject.GetComponent<Animator>();

        EventManager.Instance.Subscribe("ShieldStateEntered", HandleShieldStart);
        EventManager.Instance.Subscribe("ShieldStateExited", HandleShieldEnd);
    
    }

    private void HandleShieldStart(object eventData) => _animator.SetBool(shieldHash, true);
    private void HandleShieldEnd(object eventData) => _animator.SetBool(shieldHash, false);


}
