using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttributeType {

    Health,
    Mana

}

public class PlayerAttributes : MonoBehaviour {

    //单例实例
    private static PlayerAttributes _instance;
    public static PlayerAttributes Instance => _instance;

    [SerializeField] private static int _MaxHealth = 100;
    [SerializeField] private static int _MaxMana = 100;

    private int _currentHealth;
    private int _currentMana;
    private bool _isDead;
    private bool _deathEventTriggered;
    
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

    }

    //属性访问器
    public int Health {

        get => _currentHealth;

        private set {

            var previous = _currentHealth;
            _currentHealth = Mathf.Clamp(value, 0, _MaxHealth);
            
            if (_currentHealth <= 0 && previous > 0) {

                IsDead = true;

            }   
            
            EventManager.Instance.TriggerEvent("HealthChanged", 
                    new AttributeChangeData(
                        
                        AttributeType.Health,
                        _currentHealth,
                        _currentHealth - previous
                    
                    )
            
            );

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

    //todo
    public bool IsDead {

        get => _isDead;
        private set {

            if (_isDead != value) {

                _isDead = value;
                if (_isDead) {

                    EventManager.Instance.TriggerEvent("PlayerDied", new DeathData(Health));

                }

            }

        }

    }

    //操作方法
    //使用方式：PlayerAttributes.Instance.Takedamege(damage)
    public void TakeDamage(int amount) {

        if (IsDead) return;

        Health -= Mathf.Abs(amount);

    }

    public void Heal(int amount) {

        Health += amount;

    }

    public void UseMana(int amount) {

        Mana -= amount;

    }

    public void RestoreMana(int amount) {

        Mana += amount;

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
    
    public struct DeathData {

        public readonly int FinalHealth;
        public readonly System.DateTime DeathTime;

        public DeathData(int health) {

            FinalHealth = health;
            DeathTime = System.DateTime.Now;

        }

    }

}
