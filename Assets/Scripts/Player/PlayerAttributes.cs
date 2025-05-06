using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttributeType {

    Health,
    Mana,
    Shield,
    AttackPowerIncreased

}

public enum StateType {

    InCombat,
    Invincible,
    IsShield,
    IsRolling,
    IsDead,
    IsAttackIncreased,

}
public class PlayerAttributes : MonoBehaviour {

    //单例实例化
    private static PlayerAttributes _instance;
    public static PlayerAttributes Instance => _instance;

    [SerializeField] public int _MaxHealth = 100;
    [SerializeField] public float GetDamageInvincibleDuration = 1f;
    [SerializeField] public int _MaxMana = 15;
    [SerializeField] public float _manaRegenInterval = 1.25f;
    [SerializeField] public int _manaRegenBase = 2;
    [SerializeField] public int _MaxShield = 25;
    [SerializeField] public int _MaxAttackPowerInCreased = 100;
    private float _manaRegenTimer;

    private float _manaRegenMutiplier = 1f;
    private bool _isManaRegenPaused;

    [SerializeField] public int _currentHealth;
    [SerializeField] public int _currentMana;
    [SerializeField] public int _currentShield;
    [SerializeField] public int _currentAttackPowerInCreased;

    [SerializeField] public bool _isDead;
    private bool _deathEventTriggered;
    [SerializeField] private bool _inCombat;
    [SerializeField] private bool _isInvincible;
    [SerializeField] private bool _isRolling;

    private Coroutine _invincibleCoroutine;
    [SerializeField] private bool _hasShield;
    [SerializeField] public bool _isAttackIncreased;

    private void Awake() {

        //单例初始化
        if (_instance != null && _instance != this) {

            Destroy(gameObject);
            return;

        }

        _instance = this;
        RunStatTracker.Instance.StartTracking();

        _currentHealth = _MaxHealth;

        _currentMana = _MaxMana;
        _isManaRegenPaused = false;

        _currentShield = 0;
        _hasShield = false;

        _currentAttackPowerInCreased = 0;
        _isAttackIncreased = false;

        IsDead = false;
        _deathEventTriggered = false;

        _isInvincible = false;
        _inCombat = false;
        _isRolling = false;

    }

    //属性访问器
    public int Health {

        get => _currentHealth;

        private set {

            var previous = _currentHealth;
            _currentHealth = Mathf.Clamp(value, 0, _MaxHealth);

            EventManager.Instance.TriggerEvent("HealthChanged",
                    new AttributeChangeData(

                        AttributeType.Health,
                        _currentHealth,
                        _currentHealth - previous

                    )

            );

            if (_currentHealth <= 0 && previous > 0) {

                IsDead = true;

            }

        }

    }

    public int Mana {

        get => _currentMana;

        private set {

            var previous = _currentMana;
            _currentMana = Mathf.Clamp(value, 0, _MaxMana);

            EventManager.Instance.TriggerEvent("ManaChanged",
                    new AttributeChangeData(

                        AttributeType.Mana,
                        _currentMana,
                        _currentMana - previous

                    )

            );

        }

    }

    public int Shield {

        get => _currentShield;
        private set {

            var previous = _currentShield;
            _currentShield = Mathf.Clamp(value, 0, _MaxShield);

            // 自动更新护盾状态
            if (previous > 0 && _currentShield == 0) {

                HasShield = false;

            }

            else if (previous == 0 && _currentShield > 0) {

                HasShield = true;

            }

            EventManager.Instance.TriggerEvent("ShieldChanged",
                    new AttributeChangeData(

                        AttributeType.Shield,
                        _currentShield,
                        _currentShield - previous

                    )

            );

        }

    }

    public int AttackPowerIncreased {

        get => _currentAttackPowerInCreased;

        private set {

            var previous = _currentAttackPowerInCreased;
            _currentAttackPowerInCreased = Mathf.Clamp(value, 0, _MaxAttackPowerInCreased);

            EventManager.Instance.TriggerEvent("AttackPowerCoefficientChanged",
                    new AttributeChangeData(

                        AttributeType.AttackPowerIncreased,
                        _currentAttackPowerInCreased,
                        _currentAttackPowerInCreased - previous

                    )

            );

            if (previous > 0 && _currentAttackPowerInCreased == 0) {

                IsAttackIncreased = false;

            }

            else if (previous == 0 && _currentAttackPowerInCreased > 0) {

                CustomLogger.Log("Setpower!");
                IsAttackIncreased = true;

            }
        }

    }

    //获取Transform组件，位置/旋转信息
    public Transform PlayerTransform => GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Transform>();

    //状态访问器
    public bool IsDead {

        get => _isDead;
        private set {

            if (_isDead != value) {

                _isDead = value;

                if (_isDead && !_deathEventTriggered) {

                    _deathEventTriggered = true;
                    EventManager.Instance.TriggerEvent(

                    "PlayerDied",
                    new StateChangeData(StateType.IsDead)

                    );

                    Mana = 0;
                    AttackPowerIncreased = 0;
                    HasShield = false;
                    IsInCombat = false;
                    IsInvincible = false;

                }

            }

        }

    }
    public bool IsInCombat {

        get => _inCombat;
        private set {

            if (_inCombat != value) {

                _inCombat = value;
                EventManager.Instance.TriggerEvent(

                    value ? "InCombatStateEntered" : "InCombatStateExited",
                    new StateChangeData(StateType.InCombat)

                );

            }

        }

    }
    public bool IsInvincible {

        get => _isInvincible;
        private set {

            if (_isInvincible != value) {

                _isInvincible = value;
                EventManager.Instance.TriggerEvent(

                    value ? "InvincibleStateEntered" : "InvincibleStateExited",
                    new StateChangeData(StateType.Invincible)

                );

            }

        }

    }

    public bool IsAttackIncreased {

        get => _isAttackIncreased;
        private set {

            if (_isAttackIncreased != value) {

                _isAttackIncreased = value;
                EventManager.Instance.TriggerEvent(

                    value ? "AttackIncreasedStateEntered" : "AttackIncreasedStateExited",
                    new StateChangeData(StateType.IsAttackIncreased)

                );

            }

        }

    }

    public bool HasShield {

        get => _hasShield;
        private set {

            if (_hasShield != value) {

                _hasShield = value;
                EventManager.Instance.TriggerEvent(
                    value ? "ShieldStateEntered" : "ShieldStateExited",
                    new StateChangeData(StateType.IsShield)

                );

            }

        }

    }

    public bool IsRolling {

        get => _isRolling;
        private set {

            if (_isRolling != value) {

                _isRolling = value;
                EventManager.Instance.TriggerEvent(

                    value ? "RollingStateEntered" : "RollingStateExited",
                    new StateChangeData(StateType.IsRolling)

                );

            }

        }

    }

    //操作方法
    //使用方式：PlayerAttributes.Instance.Takedamege(damage)
    public void TakeDamage(int amount) {

        if (IsDead || IsInvincible || IsRolling)
            return;

        if (Shield > 0) {

            Shield = (Shield - amount) >= 0 ? (Shield - amount) : 0;

        } else {

            Health -= amount;

            CameraShaker.Instance.ShakeOnce(1f, 10, 0.3f, 0.2f);

            EnableInvincibleForDuration(GetDamageInvincibleDuration);

        }

    }

    //属性操作方法
    public void Heal(int amount) {

        if (Health + amount > _MaxHealth) {

            Health = _MaxHealth;

        } else {

            Health += amount;

        }

    }

    public void UseMana(int amount) {

        if (Mana - amount < 0) {

            Mana = 0;

        } else {

            Mana -= amount;

        }

    }

    public void RestoreMana(int amount) {

        if (Mana + amount > _MaxMana) {

            Mana = _MaxMana;

        } else {

            Mana += amount;

        }

    }

    public void GenerateShield() {

        Shield = _MaxShield;

    }

    public void VanishShield() {

        Shield = 0;

    }

    public void IncreaseAttackPower(int amount) {

        if (AttackPowerIncreased + amount >= _MaxAttackPowerInCreased) {

            AttackPowerIncreased = _MaxAttackPowerInCreased;

        }

        AttackPowerIncreased += amount;

    }
    public void DecreaseAttackPower(int amount) {

        if (AttackPowerIncreased - amount <= 0) {

            AttackPowerIncreased = 0;

        }

        AttackPowerIncreased -= amount;

    }

    //状态操作方法
    public void EnterCombat() => IsInCombat = true;
    public void ExitCombat() => IsInCombat = false;

    public void EnableInvincible() => IsInvincible = true;
    public void DisableInvincible() => IsInvincible = false;
    public void StartRolling()  => IsRolling = true;
    public void EndRolling() => IsRolling = false;

    public void EnableInvincibleForDuration(float duration) {

        //如果已经有正在持续的无敌协程，先停止
        if (_invincibleCoroutine != null) {

            StopCoroutine(_invincibleCoroutine);

        }

        _invincibleCoroutine = StartCoroutine(InvincibleDurationRoutine(duration));

    }

    private IEnumerator InvincibleDurationRoutine(float duration) {

        EnableInvincible();

        yield return new WaitForSeconds(duration);

        DisableInvincible();

        _invincibleCoroutine = null;

    }

    private void Update() {

        if (ShouldRegenerateMana()) {

            _manaRegenTimer += Time.deltaTime;

            if (_manaRegenTimer >= _manaRegenInterval) {

                ApplyManaRegeneration();
                _manaRegenTimer = 0f;

            }

        }

    }

    private bool ShouldRegenerateMana() {

        return !IsDead && !_isManaRegenPaused && _currentMana < _MaxMana;

    }

    private void ApplyManaRegeneration() {

        int actualRegen = Mathf.RoundToInt(_manaRegenBase * _manaRegenMutiplier);
        RestoreMana(actualRegen);

    }

    public void SetManaRegenMutiplier(float Mutiplier) {

        _manaRegenMutiplier = Mathf.Clamp(Mutiplier, 0f, 5f);

    }

    public void PauseManaRegen(bool pause) {

        _isManaRegenPaused = pause;

    }

    //属性更新数据类
    public struct AttributeChangeData {

        public readonly AttributeType Type;
        public readonly int CurrentValue;
        public readonly int CurrentAmount;

        public AttributeChangeData(AttributeType type, int current, int delta) {

            Type = type;
            CurrentValue = current;
            CurrentAmount = delta;

        }

    }

    //状态更新数据类
    public struct StateChangeData {
        public readonly StateType StateType;
       public float ChangeTime { get; set; }

        public StateChangeData(StateType type) {
            StateType = type;
           ChangeTime = Time.time;
        }

    }

}
