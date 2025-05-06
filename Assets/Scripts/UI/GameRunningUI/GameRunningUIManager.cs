using UnityEngine;
using UnityEngine.UI;

public class GameRunningUIManager : MonoBehaviour {

    [SerializeField] private Canvas PauseUI;

    public bool IsPauseUIInstantiated { get; private set; }

    public static GameRunningUIManager Instance { get; private set; }

    public void Awake() {

        if (Instance != null && Instance != this) {

            Destroy(gameObject);

        }

        Instance = this;

    }

    void Update() {

        if (!IsPauseUIInstantiated && Input.GetKeyDown(KeyCode.Escape)) {

            Instantiate(PauseUI);
            IsPauseUIInstantiated = true;

        }

    }

    public void DestroyPauseUI() {

        IsPauseUIInstantiated = false;

    }


}
