using UnityEngine;
using UnityEngine.UI;

public class CardUIDisplay : MonoBehaviour
{
    public Image cardIconImage;

    private CardDataBase cardData;
    private BackpackManager backpackManager;

    // ��BackpackManager���ã����ڳ�ʼ�����UIԪ��
    public void Setup(CardDataBase data, BackpackManager manager)
    {
        this.cardData = data;
        this.backpackManager = manager;

        cardIconImage.sprite = data.cardIcon;

        // ���ݿ����Ƿ���ӵ����������ɫ
        // ��ӵ�У�������ɫ (��ɫ)
        // δӵ�У��䰵 (��ɫ)
        cardIconImage.color = data.Owned ? Color.white : new Color(0.4f, 0.4f, 0.4f, 0.8f);
    }

    // ���������Ҫ����Inspector���ֶ��󶨵�Button��OnClick�¼�
    public void OnCardClicked()
    {
        if (cardData != null)
        {
            backpackManager.ShowCardDetail(cardData);
        }
    }
}
