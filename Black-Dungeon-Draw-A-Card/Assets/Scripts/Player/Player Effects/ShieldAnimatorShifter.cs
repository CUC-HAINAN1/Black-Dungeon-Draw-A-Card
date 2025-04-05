using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAnimatorShifter : MonoBehaviour {
    private Animator _animator;

    private void OnEnable() {
        
        EventManager.Instance.Subscribe("ShieldStateEntered", OnShieldStart);
        EventManager.Instance.Subscribe("ShieldStateExited", OnShieldEnd);
    
    }

    private void OnShieldStart(object data) => _animator.SetBool("IsShield", true);
    private void OnShieldEnd(object data) => _animator.SetBool("IsShield", false);

}
