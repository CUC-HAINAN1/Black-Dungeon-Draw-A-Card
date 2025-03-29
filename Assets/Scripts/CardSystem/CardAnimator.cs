using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CardQueueSystem))]
public class CardAnimator : MonoBehaviour 
{
    [Header("入场动画参数")] 
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private Ease positionEase = Ease.OutBack;
    [SerializeField] private float startYOffset = 1000f; // 正数 = 从下方开始

    public void PlayCardEntrance(GameObject cardObject, Transform targetSlot) 
    {
        RectTransform rt = cardObject.GetComponent<RectTransform>();
        RectTransform targetRT = targetSlot.GetComponent<RectTransform>();

        // 确保卡牌保持为目标卡槽的子对象
        rt.SetParent(targetRT, false); // false: 保持本地坐标系不变

        // 同步卡牌的缩放、旋转与目标卡槽
        rt.localScale = Vector3.one;
        rt.localRotation = targetRT.localRotation;


        // 计算起始位置（基于父级坐标系，Y轴向下偏移）
        Vector2 startPos = new Vector2(
            
            rt.anchoredPosition.x,
            rt.anchoredPosition.y - startYOffset // 下方起始点
        
        );

        // 设置初始位置
        rt.anchoredPosition = startPos;

        // 执行动画：移动到目标位置（父级坐标系原点）
        rt.DOAnchorPos(Vector2.zero, slideDuration)
            .SetEase(positionEase)
            .OnComplete(() => FinalizeCardPosition(rt));
    }

    private void FinalizeCardPosition(RectTransform rt) {
    
        rt.anchoredPosition = Vector2.zero;
    }

}