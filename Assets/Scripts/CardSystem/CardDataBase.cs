using UnityEngine;
using System;
using System.Reflection;

[CreateAssetMenu(menuName = "Card System/Card Data", order = 1)]
public class CardDataBase : ScriptableObject {

    [Header("基础设置")]
    public int cardID;
    public string displayName;
    [TextArea(3, 5)] public string description;
    public Sprite cardIcon;
    public Rarity rarity;
    public int level = 1; // 新增：卡牌等级，默认为1

    [Header("资源消耗")]
    public int manaCost;
    public float cooldown;

    [Header("玩家是否已拥有")]
    public bool Owned;

    [Header("技能类型配置")]
    public SkillType skillType;
    public InputMode inputMode;

    [Serializable]
    public class SkillBehaviorConfig {

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
    public class ProjectileParams {

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
    public class SweepParams {

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
    public class LockOnParams {

        [Tooltip("最大锁定目标数")]
        public int maxTargets;

        [Tooltip("技能伤害")]
        public int damage;

        [Tooltip("关键帧间隔")]
        public float delay;

    }

    [Serializable]
    public class AreaParams {

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
    public class InceaseAttck {

        [Tooltip("提升数值")]
        public int amount;

        [Tooltip("持续时间")]
        public float duration;

    }

    [Serializable]
    public class GenerateShield {

        [Tooltip("护盾数值")]
        public int amount;

        [Tooltip("持续时间")]
        public float duration;

    }

    [Serializable]
    public class BurstAOE {

        [Tooltip("伤害")]
        public int damage;

        [Tooltip("关键帧等待时间")]
        public float delay;

        [Tooltip("半径")]
        public float radius;

    }


    [Serializable]
    public class VisualFeedback {
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
    public class UpgradableParam {
        public string displayName;

        [Tooltip("参数路径,格式:behaviorConfig.类型.参数")]
        public string paramPath;

        public UpgradeType upgradeType;

        [Tooltip("强化值或乘数")]
        public float value;

        public enum UpgradeType { Add, Multiply }
    }

    public enum AreaShape { Circle }

    public static void UpgradeCard(CardDataBase card, CardDataBase.UpgradableParam param) {

        object current = card;
        FieldInfo field = null;

        string[] path = param.paramPath.Split('.');

        // 依次进入每一层字段
        for (int i = 0; i < path.Length; i++) {

            field = current.GetType().GetField(path[i],
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (field == null) {

                Debug.LogError($"字段未找到: {path[i]}");
                return;

            }

            // 最后一层：修改字段值
            if (i == path.Length - 1) {
                object value = field.GetValue(current);

                if (value is int intVal) {

                    int result = param.upgradeType == CardDataBase.UpgradableParam.UpgradeType.Add
                        ? intVal + (int)param.value
                        : (int)(intVal * param.value);
                    field.SetValue(current, result);

                }

                else if (value is float floatVal) {

                    float result = param.upgradeType == CardDataBase.UpgradableParam.UpgradeType.Add
                        ? floatVal + param.value
                        : floatVal * param.value;
                    field.SetValue(current, result);

                }

                else {

                    Debug.LogWarning($"字段类型不支持强化: {value.GetType()}");
                }

                return;
            }

            // 非最后一层：进入下一层嵌套
            current = field.GetValue(current);

        }

    }

    public static void CopyUpgradeFields(CardDataBase from, CardDataBase to, int cardID) {

        switch (cardID) {

            case 1:
                to.behaviorConfig.projectile.damage = from.behaviorConfig.projectile.damage;
                break;

            case 2:
                to.behaviorConfig.sweep.damage = from.behaviorConfig.sweep.damage;
                break;

            case 3:
                to.behaviorConfig.burstAOE.damage = from.behaviorConfig.burstAOE.damage;
                break;

            case 4:
                to.behaviorConfig.area.damage = from.behaviorConfig.area.damage;
                break;

            case 5:
                to.behaviorConfig.lockOn.damage = from.behaviorConfig.lockOn.damage;
                to.Owned = from.Owned;
                to.UnlockThisCard();
                break;

            case 6:
                to.behaviorConfig.increaseAttack.amount = from.behaviorConfig.increaseAttack.amount;
                break;

            case 7:

                break;

        }

    }

    /// <summary>
    /// 新增：解锁此卡牌的方法。
    /// 这个方法会去寻找场景中存在的BackpackManager实例并调用它的解锁功能。
    /// </summary>
    public void UnlockThisCard()
    {
        // 检查场景中是否有BackpackManager的实例
        if (BackpackManager.Instance != null)
        {
            BackpackManager.Instance.UnlockCardByID(this.cardID);
        }
        else
        {
            Debug.LogError("场景中找不到 BackpackManager 的实例！无法解锁卡牌。");
        }
    }

}
