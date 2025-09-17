using UnityEngine;
using UnityEngine.UI;

public class CardUIDisplay : MonoBehaviour {
    public Image cardIconImage;

    private CardDataBase cardData;
    private BackpackManager backpackManager;

    // 由BackpackManager调用，用于初始化这个UI元素
    public void Setup(CardDataBase data, BackpackManager manager) {
        this.cardData = data;
        this.backpackManager = manager;

        cardIconImage.sprite = data.cardIcon;

        // 根据卡牌是否已拥有来设置颜色
        // 已拥有：正常颜色 (白色)
        // 未拥有：变暗 (灰色)
        cardIconImage.color = data.Owned ? Color.white : new Color(0.4f, 0.4f, 0.4f, 0.8f);
    }

    // 这个方法需要你在Inspector中手动绑定到Button的OnClick事件
    public void OnCardClicked() {
        if (cardData != null) {
            backpackManager.ShowCardDetail(cardData);
        }
    }
}
