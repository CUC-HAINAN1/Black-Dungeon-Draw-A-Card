using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class SectorMeshGenerator : MonoBehaviour {
    [SerializeField] public float angle; 
    public float radius = 1f; 
    public int segments = 20; // 细分段数

    void Start() {
        
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        // 圆心顶点（枢轴点）
        vertices[0] = Vector3.zero;

        // 生成扇形边缘顶点
        float angleStep = angle * Mathf.Deg2Rad / segments;
        for (int i = 0; i <= segments; i++) {
            
            float currentAngle = angleStep * i - angle * 0.5f * Mathf.Deg2Rad;
            vertices[i + 1] = new Vector3(
                Mathf.Cos(currentAngle) * radius,
                Mathf.Sin(currentAngle) * radius,
                0
            
            );
        }

        // 生成三角形
        for (int i = 0; i < segments; i++) {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        GetComponent<MeshFilter>().mesh = mesh;
    
    }

}