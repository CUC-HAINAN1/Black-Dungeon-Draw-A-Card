using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance;
    public GameObject projectilePrefab;
    public int poolSize = 5;

    private Queue<GameObject> projectiles = new Queue<GameObject>();

    void Awake() => Instance = this;

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(projectilePrefab);
            obj.SetActive(false);
            projectiles.Enqueue(obj);
        }
    }

    public GameObject GetProjectile()
    {
        if (projectiles.Count == 0) return Instantiate(projectilePrefab);
        GameObject obj = projectiles.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public void ReturnProjectile(GameObject obj)
    {
        obj.SetActive(false);
        projectiles.Enqueue(obj);
    }
}