using UnityEngine;
using System;

public class GlobalDebugger : MonoBehaviour {

    public static GlobalDebugger Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {

            Destroy(gameObject);
            return;

        }

        else {

            Instance = this;
            DontDestroyOnLoad(this);

        }
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

                    boss.GetComponent<BossHealth>().TakeDamage(1000);

                }

            }

        }


    }


}
