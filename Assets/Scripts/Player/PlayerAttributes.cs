using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttributeType {

    Health,
    Mana,
    Shield

}

public enum StateType {

    InCombat,
    Invincible,
    IsShield,
    IsDead

}
public class PlayerAttributes : MonoBehaviour {

    //单例实例化
    private static PlayerAttributes _instance;
    public static PlayerAttributes Instance => _instance;

    [SerializeField] public int _MaxHealth = 100;
    [SerializeField] public int _MaxMana = 100;
    [SerializeField] public float _manaRegenInterval = 1f;
    [SerializeField] public int _manaRegenBase = 5;
    [SerializeField] public int _MaxShield = 25;
    private float _manaRegenTimer;
    
    private float _manaRegenMutiplier = 1f;
    private bool _isManaRegenPaused;

    [SerializeField] public int _currentHealth;
    [SerializeField] public int _currentMana;
    [SerializeField] public int _currentShield;
    
    private bool _isDead;
    private bool _deathEventTriggered;
    private bool _inCombat;
    private bool _isInvincible;
    private bool _hasShield;
    
    private void Awake() {

        //单例初始化
        if (_instance != null && _instance != this) {

            Destroy(gameObject);
            return;

        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        _currentHealth = _MaxHealth;
        
        _currentMana = _MaxMana;
        _isManaRegenPaused = false;
        
        _currentShield = _MaxShield;
        
        IsDead = false;
        _deathEventTriggered = false;

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

    //状态访问器
    public bool IsDead {

        get => _isDead;
        private set {

            if (_isDead != value) {

                _isDead = value;
                
                if (_isDead && !_deathEventTriggered) {

                    _deathEventTriggered = true;
                    EventManager.Instance.TriggerEvent("PlayerDied", new StateChangeData(StateType.IsDead));

                    _currentMana = 0;

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

    //操作方法
    //使用方式：PlayerAttributes.Instance.Takedamege(damage)
    public void TakeDamage(int amount) {

        if (IsDead) return;

        if (_currentShield > 0) {

            _currentShield -= amount;

        } else {

            _currentHealth -= amount;

        } 

    }

    //属性操作方法
    public void Heal(int amount) {

        Health += amount;

    }

    public void UseMana(int amount) {

        Mana -= amount;

    }

    public void RestoreMana(int amount) {

        Mana += amount;

    }

    public void GenerateShield() {

        _currentShield = _MaxShield;

    }

    public void VanishShield() {

        _currentShield = 0;

    }

    //状态操作方法
    public void EnterCombat() => IsInCombat = true;
    public void ExitCombat() => IsInCombat = false;

    public void EnableInvincible() => IsInvincible = true;
    public void DisableInvincible() => IsInvincible = false;
        
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
        _currentMana += actualRegen;

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
        public readonly DateTime ChangeTime;

        public StateChangeData(StateType type) {
            
            StateType = type;
            ChangeTime = DateTime.Now;
        
        }
    
    }

}
