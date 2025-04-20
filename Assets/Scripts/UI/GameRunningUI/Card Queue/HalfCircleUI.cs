using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Half Circle")]
public class HalfCircleUI : MaskableGraphic {

    [Range(0, 360)]public float arcAngle = 180f;
    public int segments = 50;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        
        Rect rect = GetPixelAdjustedRect();
        Vector2 center = rect.center;
        float radius = Mathf.Min(rect.width, rect.height) * 0.5f;
        float angleStep = arcAngle / segments;

        // 添加中心顶点（用于三角剖分）
        UIVertex centerVert = new UIVertex();
        centerVert.position = center;
        centerVert.color = color;
        vh.AddVert(centerVert);

        // 生成圆弧顶点
        for (int i = 0; i <= segments; i++)
        {
            float angle = -arcAngle / 2 + angleStep * i;
            Vector2 pos = center + new Vector2(
                Mathf.Cos(Mathf.Deg2Rad * angle) * radius,
                Mathf.Sin(Mathf.Deg2Rad * angle) * radius
            );
            
            UIVertex vert = new UIVertex();
            vert.position = pos;
            vert.color = color;
            vh.AddVert(vert);
        }

        // 生成三角形（扇形）
        for (int i = 1; i <= segments; i++)
        {
            vh.AddTriangle(0, i, i + 1);
        }
    }
   
}
