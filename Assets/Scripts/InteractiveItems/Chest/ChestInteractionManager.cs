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

                CustomLogger.Log("首次获得雷电卡牌！");
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

        CustomLogger.LogWarning($"强化：{upgrade.paramPath} -> {upgrade.upgradeType} {upgrade.value}");

        RewardUIManager.Instance.ShowUpgradeUI(card, upgrade);

        CardDataBase.UpgradeCard(card, upgrade);

    }

}
