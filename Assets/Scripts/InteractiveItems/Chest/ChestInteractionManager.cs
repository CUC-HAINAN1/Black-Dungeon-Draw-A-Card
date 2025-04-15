using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class ChestInteraction : MonoBehaviour {
    public Animator animator;
    public float interactionRange = 100f;
    private string openAnimationName = "treasure_0";
    public GameObject interactionUI;

    private bool isOpened = false;
    private Transform playerTransform;


    void Start() {

        animator = gameObject.GetComponentInChildren<Animator>();
        animator.enabled = false;

        if (interactionUI != null) {

            interactionUI.SetActive(false);

        }

    }

    void Update() {

        if (isOpened || PlayerAttributes.Instance == null)
            return;

        playerTransform = PlayerAttributes.Instance.PlayerTransform;

        Vector3 chestPos = transform.position;
        Vector3 playerPos = playerTransform.position;

        chestPos.z = 0f;
        playerPos.z = 0f;

        float distance = Vector2.Distance(chestPos, playerPos);

        if (distance <= interactionRange) {

            if (interactionUI != null && !interactionUI.activeSelf) {

                interactionUI.SetActive(true);

            }

            if (Input.GetKeyDown(KeyCode.E)) {

                StartCoroutine(OpenChestCoroutine());

            }
        }

        else {

            if (interactionUI != null && interactionUI.activeSelf) {

                interactionUI.SetActive(false);

            }
        }
    }


    IEnumerator OpenChestCoroutine() {

        isOpened = true;
        animator.enabled = true;

        animator.Play(openAnimationName);

        yield return new WaitForSeconds(0.4f);
        animator.enabled = false;


        GetReward();

        if (interactionUI != null) {

            interactionUI.SetActive(false);

        }

    }

    void GetReward() {

        var rewardPoolManager = RewardPoolManager.Instance;

        var reward = rewardPoolManager.GetRewardForChest(rewardPoolManager.curRoomIndex++);

        if (reward.isSpecial) {

            if (!reward.card.Owned) {

                // 玩家未拥有雷电卡，设置为已拥有
                reward.card.Owned = true;
                CardPoolManager.Instance.AddCardToPool(reward.card);

                Debug.Log("首次获得雷电卡牌！");
                RewardUIManager.Instance.ShowNewCardUI(reward.card);

            }

            else {
                // 已拥有雷电卡，派发强化
                ApplyUpgrade(reward.card);

            }

        }

        else {

            ApplyUpgrade(reward.card);

        }

    }

    private void ApplyUpgrade(CardDataBase card) {

        var upgrade = card.upgradableParams[0];

        Debug.Log($"强化：{upgrade.paramPath} -> {upgrade.upgradeType} {upgrade.value}");

        RewardUIManager.Instance.ShowUpgradeUI(card, upgrade);

        UpgradeHelper(card, upgrade);

    }

    private void UpgradeHelper(CardDataBase card, CardDataBase.UpgradableParam param) {

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

}
