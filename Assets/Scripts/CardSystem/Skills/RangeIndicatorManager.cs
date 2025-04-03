using UnityEngine;

public class RangeIndicatorManager : MonoBehaviour {
    
    public static RangeIndicatorManager Instance { get; private set; }
    private IRangeIndicator currentIndicator;
    private PlayerAttributes playerAttributes;

    public void Awake() {
        
        if (Instance != null && Instance != this) {
        
            Destroy(gameObject);
        
        }
        else {
        
            Instance = this;
        
        }

        playerAttributes = PlayerAttributes.Instance;
        
    }

    public void CreateIndicator(CardDataBase cardData) {
        
        // 清理旧指示器
        if(currentIndicator != null) {
            
            currentIndicator.Terminate();
        
        }
        
        if (cardData.visualConfig.rangeIndicatorPrefab == null) return;

        Debug.Log(cardData.displayName);

        if (playerAttributes == null) Debug.Log("position");
        if (Quaternion.identity == null) Debug.Log("identity");

        // 实例化新指示器
        GameObject indicatorObj = Instantiate(
                
                cardData.visualConfig.rangeIndicatorPrefab,
                playerAttributes.PlayerTransform.position,
                Quaternion.identity
        
        );
        if (indicatorObj == null) {

            Debug.Log("Invalid indicatorObj!");
            return;

        }
        
        
        currentIndicator = indicatorObj.GetComponent<IRangeIndicator>();
        
        if (currentIndicator == null) return;
        
        currentIndicator.Initialize(cardData);
    
    }

    public void UpdateIndicator(Vector2 delta) {
        
        currentIndicator?.UpdateIndicator(delta);
    
    }

    public void ClearIndicator() {
        
        currentIndicator?.Terminate();
        currentIndicator = null;
    
    }

}