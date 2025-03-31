using UnityEngine;
public class ShieldController : MonoBehaviour
{
    [Header("Shield Settings")]
    [SerializeField] private KeyCode shieldKey = KeyCode.U; // 触发按键

    private void Update()
    {
        HandleShieldInput();
    }

    private void HandleShieldInput()
    {
        if (Input.GetKeyDown(shieldKey) && CanActivateShield())
        {
            ActivateShield();
        }
    }

    private bool CanActivateShield()
    {
        // 验证条件：非死亡状态 + 当前没有护盾
        return !PlayerAttributes.Instance.IsDead && 
               PlayerAttributes.Instance.Shield <= 0;
    }

    private void ActivateShield()
    {
        // 生成护盾
        PlayerAttributes.Instance.GenerateShield();
        
    }

}