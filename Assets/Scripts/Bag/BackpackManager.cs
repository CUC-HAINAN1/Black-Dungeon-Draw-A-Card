using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting; // ���ڹ����ַ���

public class BackpackManager : MonoBehaviour
{
    [Header("��������")]
    [Tooltip("������Ŀ�����е�CardDataBase�ʲ��ϵ�����")]
    public List<CardDataBase> allCardsDatabase;

    [Header("UI ���� - ������")]
    public GameObject bagPanel;
    public Button bagButton;
    public Transform cardContainer;
    public Button leftPageButton;
    public Button rightPageButton;
    public Text pageInfoText; // ��ѡ��������ʾ "1 / 3"

    [Header("UI ���� - Ԥ�Ƽ�")]
    public GameObject cardDisplayPrefab; // ��� CardDisplay_Prefab   ����չʾ���Ƶ�Ԥ����

    [Header("UI ���� - �������")]
    public GameObject cardDetailPanel;
    public Button closeDetailButton;
    public Image detailIcon;
    public Text detailNameText;
    public Text detailRarityText;
    public Text detailDescriptionText;
    // --- ����������������� ---
    public Text detailManaCostText; // ������������ʾ��������
    public Text detailCooldownText; // ������������ʾ��ȴʱ��
    

    [Header("��������")]
    public int cardsPerPage = 12;//ÿҳ���ٿ���
    //���ñ�����ʼչʾ״̬
    private int currentPage = 0;
    private bool isBagOpen = false;

    // --- ����������ģʽ ---
    // ����һ����̬ʵ�����������κνű�������ͨ�� BackpackManager.Instance ��������
    public static BackpackManager Instance { get; private set; }

    // Awake �� Start ֮ǰ������
    void Awake()
    {
        // --- ���������õ��� ---
        // ȷ��������ֻ��һ��BackpackManagerʵ��
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        // ȷ��UI�ڿ�ʼʱ�ǹرյ�  ��ʾUI������SetActive
        bagPanel.SetActive(false);
        cardDetailPanel.SetActive(false);


        //��UI����ת���ʱ�����ʹ������¼��ķ�ʽ

        // �󶨰�ť�¼�   �ڰ��°�����ʱ������¼�
        bagButton.onClick.AddListener(ToggleBag);
        //�������һ�ҳ  ֱ�ӵ��ú���
        leftPageButton.onClick.AddListener(() => ChangePage(-1));
        rightPageButton.onClick.AddListener(() => ChangePage(1));
        //���¹رհ�ť   ��ӹر��¼�
        closeDetailButton.onClick.AddListener(CloseCardDetailPanel);
    }

    // Update is called once per frame
    void Update()
    {
        // ����Tab��   ����   �������ֱ�ӵ���
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleBag();
        }
    }

    public void ToggleBag()
    {//��ת���״̬  ����boolֵ
        isBagOpen = !isBagOpen;
        bagPanel.SetActive(isBagOpen);//״̬�ɹص���

        if (cardDetailPanel.activeSelf)
        {
            //��֤��������ر�״̬   �ȵ��ùرպ���
            CloseCardDetailPanel();
        }

        Time.timeScale = isBagOpen ? 0f : 1f;

        if (isBagOpen)//ÿһ�ε��ú���������״̬��֤
        {
            // �򿪱���ʱ�����õ���һҳ��ˢ����ʾ
            currentPage = 0;
            DisplayCardsForCurrentPage();
        }
    }

    private void DisplayCardsForCurrentPage()
    {
        // 1. ���������оɵĿ���ͼ��  ��ȡ������
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. ���㵱ǰҳ�Ŀ��Ʒ�Χ
        int startIndex = currentPage * cardsPerPage;
        int endIndex = Mathf.Min(startIndex + cardsPerPage, allCardsDatabase.Count);

        // 3. ʵ������ǰҳ�Ŀ���ͼ��
        for (int i = startIndex; i < endIndex; i++)
        {
            CardDataBase currentCard = allCardsDatabase[i];
            //����һ��ʵ����cardDisplayPrefab   �������вٿغ�����   Ԥ�����������������ݵ�
            GameObject cardObj = Instantiate(cardDisplayPrefab, cardContainer);//����һ������Ԥ����  ��ΪCardContainer��������
            CardUIDisplay display = cardObj.GetComponent<CardUIDisplay>();

            if (display != null)
            {
                display.Setup(currentCard, this);
            }
        }
        // 4. ���·�ҳ��ť��ҳ����Ϣ
        UpdatePageControls();
    }

    private void ChangePage(int direction)
    {//��ȡ��ҳ��
        int totalPages = Mathf.CeilToInt((float)allCardsDatabase.Count / cardsPerPage);
        currentPage += direction;

        // ˢ����ʾ
        DisplayCardsForCurrentPage();
    }

    private void UpdatePageControls()
    {
        int totalPages = Mathf.CeilToInt((float)allCardsDatabase.Count / cardsPerPage);
        //��ֹ�������
        if (totalPages <= 0) totalPages = 1; // ������1ҳ
        // ���ݵ�ǰҳ������Ƿ���Ե����ҳ��ť    �жϰ�ť�ܷ񽻻�
        leftPageButton.interactable = (currentPage > 0);//�����ж�����
        rightPageButton.interactable = (currentPage < totalPages - 1);//��ǰҳ���Ѿ��������һҳ��

        //�޸���ʾ��ǰ�ڼ�ҳ
        if (pageInfoText != null)
        {
            pageInfoText.text = $"{currentPage + 1} / {totalPages}";
        }
    }

    // ������������ʾ��������
    public void ShowCardDetail(CardDataBase data)
    {
        cardDetailPanel.SetActive(true);

        detailIcon.sprite = data.cardIcon;//ͼ��
        detailNameText.text = data.displayName;//����
        detailRarityText.text = data.rarity.ToString();//ϡ�ж�
        detailRarityText.color = GetRarityColor(data.rarity); // ����ϡ�жȸ�������ɫ
        detailDescriptionText.text = data.description;//��������

        // ʹ��StringBuilder����Ч�ع��������ַ���
        StringBuilder statsBuilder = new StringBuilder();
        statsBuilder.AppendLine($"��������: {data.manaCost}");//�ϲ��ַ���
        statsBuilder.AppendLine($"��ȴʱ��: {data.cooldown}s");
        // �������������Ӹ�������չʾ������...

        detailManaCostText.text = $"��������: {data.manaCost}";
        detailCooldownText.text = $"��ȴʱ��: {data.cooldown}s"; // ����ȴʱ������ "s" ��ʾ��
    }

    // �����������رտ�������
    public void CloseCardDetailPanel()
    {
        cardDetailPanel.SetActive(false);
    }

    // ��������������ϡ�жȷ��ز�ͬ��ɫ
    private Color GetRarityColor(CardDataBase.Rarity rarity)
    {
        switch (rarity)
        {
            case CardDataBase.Rarity.Common:
                return Color.white;//��������
            case CardDataBase.Rarity.Rare:
                return Color.cyan; // ϡ�п�������ɫ
            // ������Ӹ���ϡ�ж�...
            default:
                return Color.gray;
        }
    }


    // --- ��Ҫ����ν������� ---
    // �ⲿϵͳ���������̵꣩����ͨ��ID�����������������һ�ſ���
    public void UnlockCardByID(int cardID)
    {
        CardDataBase cardToUnlock = allCardsDatabase.Find(card => card.cardID == cardID);
        if (cardToUnlock != null)
        {
            cardToUnlock.Owned = true;
            Debug.Log($"�����ѽ���: {cardToUnlock.displayName}");

            // ��������Ǵ򿪵ģ�ˢ��һ����ʾ�����½����Ŀ��Ʊ���
            if (isBagOpen)
            {
                DisplayCardsForCurrentPage();
            }
        }
        else
        {
            Debug.LogWarning($"δ�ҵ�IDΪ {cardID} �Ŀ��ơ�");
        }
    }
}
