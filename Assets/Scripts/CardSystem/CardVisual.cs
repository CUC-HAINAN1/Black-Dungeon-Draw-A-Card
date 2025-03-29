using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private Image cardImage; 
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void Initialize(CardDataBase data)
    {
        
        cardImage.sprite = data.cardIcon;
        nameText.text = data.displayName;
        descriptionText.text = data.description;
        
        // 根据稀有度改变边框颜色
        GetComponent<Image>().color = data.rarity == CardDataBase.Rarity.Rare 
            ? Color.yellow 
            : Color.white;
    
    }

}