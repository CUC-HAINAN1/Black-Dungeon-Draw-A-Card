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

            // �����ſ�����
            DoorStateController controller = doorDirection.gameObject.AddComponent<DoorStateController>();

            // �����
            controller.upDoor = FindChild(doorDirection, "UpDoors");
            controller.downDoor = FindChild(doorDirection, "DownDoors");

            // �Ƴ��� Start() ���ֶ����ã���Ϊ Unity ���Զ����� MonoBehaviour ���������ڷ���
        }
    }

    GameObject FindChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Equals(name))
                return child.gameObject;
        }
        CustomLogger.LogError($"�Ӷ���ȱʧ: {parent.name}/{name}");
        return null;
    }
}
