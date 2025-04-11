using UnityEngine;
using System;
using UnityEditor.EditorTools;

[CreateAssetMenu(menuName = "Card System/Card Data", order = 1)]
public class CardDataBase : ScriptableObject {

    [Header("基础设置")]
    public int cardID;
    public string displayName;
    [TextArea(3, 5)] public string description;
    public Sprite cardIcon;
    public Rarity rarity;

    [Header("资源消耗")]
    public int manaCost;
    public float cooldown;

    [Header("玩家是否已拥有")]
    public bool Owned;

    [Header("技能类型配置")]
    public SkillType skillType;
    public InputMode inputMode;

    [Serializable]
    public struct SkillBehaviorConfig {

        [Header("弹道类设置")]
        public ProjectileParams projectile;

        [Header("扇形攻击设置")]
        public SweepParams sweep;

        [Header("锁定类设置")]
        public LockOnParams lockOn;

        [Header("区域类设置")]
        public AreaParams area;

        [Header("伤害增益类设置")]
        public InceaseAttck increaseAttack;

        [Header("护盾生成设置")]
        public GenerateShield generateShield;

        [Header("单次群体AOE类设置")]
        public BurstAOE burstAOE;

    }
    public SkillBehaviorConfig behaviorConfig;

    [Header("视觉反馈")]
    public VisualFeedback visualConfig;

    [Header("升级参数")]
    public UpgradableParam[] upgradableParams;

    // 以下是嵌套结构体定义
    public enum Rarity {
        Common,
        Rare,
    }

    public enum SkillType {
        Projectile,
        Sweep,
        LockOn,
        Area,
        InceaseAttck,
        GenerateShield,
        BurstAOE
    }

    public enum InputMode {
        DragDirection,
        TargetLock,
        AreaSelection,
        HoldRelease
    }


    [Serializable]
    public struct ProjectileParams {

        [Tooltip("飞行速度(米/秒)")]
        public float speed;

        [Tooltip("同时发射数量")]
        public int projectileCount;

        [Tooltip("最大射程")]
        public float maxRange;

        [Tooltip("发射间隔")]
        public float interval;

        [Tooltip("单个特效伤害")]
        public int damage;

    }

    [Serializable]
    public struct SweepParams {

        [Tooltip("扇形角度")]
        [Range(5, 360)] public float angle;

        [Tooltip("作用半径")]
        public float radius;

        [Tooltip("技能伤害")]
        public int damage;

        [Tooltip("关键帧间隔")]
        public float delay;

        [Tooltip("Y轴心点偏移")]
        public float pivotOffsetY;

        [Tooltip("X轴心点偏移")]
        public float pivotOffsetX;

    }

    [Serializable]
    public struct LockOnParams {

        [Tooltip("最大锁定目标数")]
        public int maxTargets;

        [Tooltip("技能伤害")]
        public int damage;

        [Tooltip("关键帧间隔")]
        public float delay;

    }

    [Serializable]
    public struct AreaParams {

        [Tooltip("圆形半径")]
        public float radius;

        [Tooltip("持续时间")]
        public float duration;

        [Tooltip("每秒伤害次数")]
        public float tickRate;

        [Tooltip("单次伤害")]
        public int damage;

    }

    [Serializable]
    public struct InceaseAttck {

        [Tooltip("提升数值")]
        public int amount;

        [Tooltip("持续时间")]
        public float duration;

    }

    [Serializable]
    public struct GenerateShield {

        [Tooltip("护盾数值")]
        public int amount;

        [Tooltip("持续时间")]
        public float duration;

    }

    [Serializable]
    public struct BurstAOE {

        [Tooltip("伤害")]
        public int damage;

        [Tooltip("关键帧等待时间")]
        public float delay;

        [Tooltip("半径")]
        public float radius;

    }


    [Serializable]
    public struct VisualFeedback {
        [Header("预览效果")]
        public GameObject rangeIndicatorPrefab;

        [Header("施法效果")]
        public GameObject castEffect;
        public AudioClip castSound;

        [Header("命中效果")]
        public GameObject hitEffect;
        public AudioClip hitSound;
    }

    [Serializable]
    public struct UpgradableParam {
        public string displayName;

        [Tooltip("参数路径,格式:behaviorConfig.类型.参数")]
        public string paramPath;

        public UpgradeType upgradeType;

        [Tooltip("强化值或乘数")]
        public float value;

        public enum UpgradeType { Add, Multiply }
    }

    public enum AreaShape { Circle }

}
