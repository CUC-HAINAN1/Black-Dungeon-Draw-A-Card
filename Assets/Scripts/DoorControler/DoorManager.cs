using UnityEngine;

public class DoorManager : MonoBehaviour
{
    void Start()
    {
        // ��̬���Ҳ����������ŷ���VerDownDoor RIGHT/LEFT��
        ConfigureAllDoors();
    }

    void ConfigureAllDoors()
    {
        // ���������������ŷ���VerDownDoor RIGHT �� VerDownDoor LEFT��
        foreach (Transform doorDirection in transform)
        {
            string directionName = doorDirection.name;
            if (!directionName.Contains("VerDownDoor")) continue; // ���˷��Ŷ���

            // ��̬����ſ��������
            DoorStateController controller = doorDirection.gameObject.AddComponent<DoorStateController>();

            // ��̬�� UpDoors �� DownDoors
            controller.upDoor = FindChild(doorDirection, "UpDoors");
            controller.downDoor = FindChild(doorDirection, "DownDoors");

            // ��ʼ����״̬
            controller.Start();
        }
    }

    GameObject FindChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child.gameObject;
        }
        Debug.LogError($"δ�ҵ��Ӷ���: {parent.name}/{name}");
        return null;
    }
}
