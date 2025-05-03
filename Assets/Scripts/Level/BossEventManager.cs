using UnityEngine;
using System;

public class BossEventManager : MonoBehaviour {

    private BossCameraMovement bossCameraMovement;

    public static BossEventManager Instance { get; private set; }

    private void Awake() {

        if (Instance != null && Instance != this) {

            Destroy(gameObject);
            return;

        }

        Instance = this;
        Initialize();

    }

    private void Initialize() {

        bossCameraMovement = GetComponent<BossCameraMovement>();

    }

    public void PlayBossCinematic(Transform bossTransform) {

        bossCameraMovement.PlayCinematic(bossTransform);

    }

}
