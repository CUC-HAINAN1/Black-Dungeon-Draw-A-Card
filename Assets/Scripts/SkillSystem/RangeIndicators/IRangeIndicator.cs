public interface IRangeIndicator {
    void Initialize(CardDataBase cardData);  // 初始化方法
    void UpdateIndicator(); // 拖拽时更新
    void Terminate(); // 结束指示
    T GetContext <T> () where T : struct; //泛型获取技能所需数据

}

