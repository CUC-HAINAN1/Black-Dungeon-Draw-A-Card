using UnityEngine;

public class RangeIndicatorManager : MonoBehaviour {

    public static RangeIndicatorManager Instance { get; private set; }
    private IRangeIndicator currentIndicator;
    private PlayerAttributes playerAttributes;

    void Awake() {

        if (Instance != null && Instance != this) {

            Destroy(gameObject);
            return;

        }

        Instance = this;

    }

    public void CreateIndicator(CardDataBase cardData) {

        playerAttributes = PlayerAttributes.Instance;

        // 清理旧指示器
        if(currentIndicator != null) {

            currentIndicator.Terminate();

        }

        if (cardData.visualConfig.rangeIndicatorPrefab == null) return;

        Transform playerTransform = playerAttributes.PlayerTransform;

        // 实例化新指示器
        GameObject indicatorObj = Instantiate(

                cardData.visualConfig.rangeIndicatorPrefab,
                playerTransform.position,
                Quaternion.identity,
                playerTransform

        );

        if (indicatorObj == null) {

            Debug.Log("Invalid indicatorObj!");
            return;

        }

        currentIndicator = indicatorObj.GetComponent<IRangeIndicator>();

        if (currentIndicator == null) return;

        currentIndicator.Initialize(cardData);

    }

    public void UpdateIndicator() {

        currentIndicator?.UpdateIndicator();

    }

    public void ClearIndicator() {

        currentIndicator?.Terminate();
        currentIndicator = null;

    }

    public T GetContext <T> () {

        return currentIndicator.GetContext<T>();

    }

}
