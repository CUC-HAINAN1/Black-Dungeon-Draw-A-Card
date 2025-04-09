using UnityEngine;

public class DoorManager : MonoBehaviour
{
    void Start()
    {
        ConfigureAllDoors();
    }

    void ConfigureAllDoors()
    {
        foreach (Transform doorDirection in transform)
        {
            string directionName = doorDirection.name;
            if (!directionName.Contains("VerDownDoor")) continue;

            // 添加门控制器
            DoorStateController controller = doorDirection.gameObject.AddComponent<DoorStateController>();

            // 绑定组件
            controller.upDoor = FindChild(doorDirection, "UpDoors");
            controller.downDoor = FindChild(doorDirection, "DownDoors");

            // 移除了 Start() 的手动调用，因为 Unity 会自动调用 MonoBehaviour 的生命周期方法
        }
    }

    GameObject FindChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Equals(name))
                return child.gameObject;
        }
        Debug.LogError($"子对象缺失: {parent.name}/{name}");
        return null;
    }
}
