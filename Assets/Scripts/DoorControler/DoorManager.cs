using UnityEngine;

public class DoorManager : MonoBehaviour
{
    void Start()
    {
        // 动态查找并配置所有门方向（VerDownDoor RIGHT/LEFT）
        ConfigureAllDoors();
    }

    void ConfigureAllDoors()
    {
        // 遍历房间内所有门方向（VerDownDoor RIGHT 和 VerDownDoor LEFT）
        foreach (Transform doorDirection in transform)
        {
            string directionName = doorDirection.name;
            if (!directionName.Contains("VerDownDoor")) continue; // 过滤非门对象

            // 动态添加门控制器组件
            DoorStateController controller = doorDirection.gameObject.AddComponent<DoorStateController>();

            // 动态绑定 UpDoors 和 DownDoors
            controller.upDoor = FindChild(doorDirection, "UpDoors");
            controller.downDoor = FindChild(doorDirection, "DownDoors");

            // 初始化门状态
            controller.Start();
        }
    }

    GameObject FindChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child.gameObject;
        }
        Debug.LogError($"未找到子对象: {parent.name}/{name}");
        return null;
    }
}
