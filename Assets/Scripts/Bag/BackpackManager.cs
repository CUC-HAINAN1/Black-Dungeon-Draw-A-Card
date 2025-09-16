using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting; // 用于构建字符串

public class BackpackManager : MonoBehaviour
{
    [Header("核心数据")]
    [Tooltip("将你项目中所有的CardDataBase资产拖到这里")]
    public List<CardDataBase> allCardsDatabase;

    [Header("UI 引用 - 主背包")]
    public GameObject bagPanel;
    public Button bagButton;
    public Transform cardContainer;
    public Button leftPageButton;
    public Button rightPageButton;
    public Text pageInfoText; // 可选，用于显示 "1 / 3"

    [Header("UI 引用 - 预制件")]
    public GameObject cardDisplayPrefab; // 你的 CardDisplay_Prefab   用来展示卡牌的预制体

    [Header("UI 引用 - 详情面板")]
    public GameObject cardDetailPanel;
    public Button closeDetailButton;
    public Image detailIcon;
    public Text detailNameText;
    public Text detailRarityText;
    public Text detailDescriptionText;
    // --- 在这里添加下面两行 ---
    public Text detailManaCostText; // 新增：用于显示法力消耗
    public Text detailCooldownText; // 新增：用于显示冷却时间
    

    [Header("背包设置")]
    public int cardsPerPage = 12;//每页多少卡牌
    //设置背包初始展示状态
    private int currentPage = 0;
    private bool isBagOpen = false;

    // --- 新增：单例模式 ---
    // 创建一个静态实例，让其他任何脚本都可以通过 BackpackManager.Instance 来访问它
    public static BackpackManager Instance { get; private set; }

    // Awake 在 Start 之前被调用
    void Awake()
    {
        // --- 新增：设置单例 ---
        // 确保场景中只有一个BackpackManager实例
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
        // 确保UI在开始时是关闭的  显示UI就是用SetActive
        bagPanel.SetActive(false);
        cardDetailPanel.SetActive(false);


        //在UI界面转变的时候可以使用添加事件的方式

        // 绑定按钮事件   在按下按键的时候添加事件
        bagButton.onClick.AddListener(ToggleBag);
        //按下左右换页  直接调用函数
        leftPageButton.onClick.AddListener(() => ChangePage(-1));
        rightPageButton.onClick.AddListener(() => ChangePage(1));
        //按下关闭按钮   添加关闭事件
        closeDetailButton.onClick.AddListener(CloseCardDetailPanel);
    }

    // Update is called once per frame
    void Update()
    {
        // 监听Tab键   监听   如果按下直接调用
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleBag();
        }
    }

    public void ToggleBag()
    {//先转变打开状态  调整bool值
        isBagOpen = !isBagOpen;
        bagPanel.SetActive(isBagOpen);//状态由关到开

        if (cardDetailPanel.activeSelf)
        {
            //保证二级界面关闭状态   先调用关闭函数
            CloseCardDetailPanel();
        }

        Time.timeScale = isBagOpen ? 0f : 1f;

        if (isBagOpen)//每一次调用函数都进行状态保证
        {
            // 打开背包时，重置到第一页并刷新显示
            currentPage = 0;
            DisplayCardsForCurrentPage();
        }
    }

    private void DisplayCardsForCurrentPage()
    {
        // 1. 清理容器中旧的卡牌图标  获取子物体
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. 计算当前页的卡牌范围
        int startIndex = currentPage * cardsPerPage;
        int endIndex = Mathf.Min(startIndex + cardsPerPage, allCardsDatabase.Count);

        // 3. 实例化当前页的卡牌图标
        for (int i = startIndex; i < endIndex; i++)
        {
            CardDataBase currentCard = allCardsDatabase[i];
            //创建一个实例的cardDisplayPrefab   用来进行操控和设置   预制体是用来传入数据的
            GameObject cardObj = Instantiate(cardDisplayPrefab, cardContainer);//创建一个卡牌预制体  作为CardContainer的子物体
            CardUIDisplay display = cardObj.GetComponent<CardUIDisplay>();

            if (display != null)
            {
                display.Setup(currentCard, this);
            }
        }
        // 4. 更新翻页按钮和页码信息
        UpdatePageControls();
    }

    private void ChangePage(int direction)
    {//获取总页数
        int totalPages = Mathf.CeilToInt((float)allCardsDatabase.Count / cardsPerPage);
        currentPage += direction;

        // 刷新显示
        DisplayCardsForCurrentPage();
    }

    private void UpdatePageControls()
    {
        int totalPages = Mathf.CeilToInt((float)allCardsDatabase.Count / cardsPerPage);
        //防止特殊情况
        if (totalPages <= 0) totalPages = 1; // 至少有1页
        // 根据当前页码决定是否可以点击翻页按钮    判断按钮能否交互
        leftPageButton.interactable = (currentPage > 0);//给出判定条件
        rightPageButton.interactable = (currentPage < totalPages - 1);//当前页面已经到了最后一页了

        //修改显示当前第几页
        if (pageInfoText != null)
        {
            pageInfoText.text = $"{currentPage + 1} / {totalPages}";
        }
    }

    // 公开方法：显示卡牌详情
    public void ShowCardDetail(CardDataBase data)
    {
        cardDetailPanel.SetActive(true);

        detailIcon.sprite = data.cardIcon;//图标
        detailNameText.text = data.displayName;//名字
        detailRarityText.text = data.rarity.ToString();//稀有度
        detailRarityText.color = GetRarityColor(data.rarity); // 根据稀有度给文字上色
        detailDescriptionText.text = data.description;//技能描述

        // 使用StringBuilder来高效地构建属性字符串
        StringBuilder statsBuilder = new StringBuilder();
        statsBuilder.AppendLine($"法力消耗: {data.manaCost}");//合并字符串
        statsBuilder.AppendLine($"冷却时间: {data.cooldown}s");
        // 你可以在这里添加更多你想展示的属性...

        detailManaCostText.text = $"法力消耗: {data.manaCost}";
        detailCooldownText.text = $"冷却时间: {data.cooldown}s"; // 在冷却时间后加上 "s" 表示秒
    }

    // 公开方法：关闭卡牌详情
    public void CloseCardDetailPanel()
    {
        cardDetailPanel.SetActive(false);
    }

    // 辅助方法：根据稀有度返回不同颜色
    private Color GetRarityColor(CardDataBase.Rarity rarity)
    {
        switch (rarity)
        {
            case CardDataBase.Rarity.Common:
                return Color.white;//正常卡牌
            case CardDataBase.Rarity.Rare:
                return Color.cyan; // 稀有卡牌用青色
            // 可以添加更多稀有度...
            default:
                return Color.gray;
        }
    }


    // --- 重要：如何解锁卡牌 ---
    // 外部系统（如任务、商店）可以通过ID调用这个方法来解锁一张卡牌
    public void UnlockCardByID(int cardID)
    {
        CardDataBase cardToUnlock = allCardsDatabase.Find(card => card.cardID == cardID);
        if (cardToUnlock != null)
        {
            cardToUnlock.Owned = true;
            Debug.Log($"卡牌已解锁: {cardToUnlock.displayName}");

            // 如果背包是打开的，刷新一下显示，让新解锁的卡牌变亮
            if (isBagOpen)
            {
                DisplayCardsForCurrentPage();
            }
        }
        else
        {
            Debug.LogWarning($"未找到ID为 {cardID} 的卡牌。");
        }
    }
}
