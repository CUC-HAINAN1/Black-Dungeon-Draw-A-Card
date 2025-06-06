using UnityEngine;
using System;

public class GlobalDebugger : MonoBehaviour {

    public static GlobalDebugger Instance { get; private set; }

    private bool Kaigua = false;

    private void Awake() {

        if (Instance != null && Instance != this) {

            Destroy(gameObject);
            return;

        }

        Instance = this;
        DontDestroyOnLoad(this);

    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.P)) {

            var playerAttributes = PlayerAttributes.Instance;
            if (playerAttributes != null) {

                playerAttributes.TakeDamage(200);

            }

        }

        if (Input.GetKeyDown(KeyCode.L)) {

            var eventManager = EventManager.Instance;

            if (eventManager != null) {

                eventManager.TriggerEvent("BossDied");

            }

        }

        if (Input.GetKeyDown(KeyCode.M)) {

            var eventManager = EventManager.Instance;

            if (eventManager != null) {

                var boss = GameObject.FindGameObjectWithTag("Boss");
                if (boss != null) {

                    boss.GetComponent<BossHealth>().TakeDamage(10000);

                }

            }

        }

        if (Input.GetKeyDown(KeyCode.K)) {

            var playerAttributes = PlayerAttributes.Instance;

            if (playerAttributes != null) {

                if (Kaigua) {

                    Kaigua = false;
                    playerAttributes.DecreaseAttackPower(100);
                    playerAttributes.DisableInvincible();

                }
                else {

                    Kaigua = true;
                    playerAttributes.IncreaseAttackPower(100);
                    playerAttributes.EnableInvincible();

                }

            }

        }


    }


}
