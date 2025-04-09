using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Edgar.Unity; // RoomBase �ڴ������ռ���

[CreateAssetMenu(menuName = "Custom/CustomRoom", fileName = "New CustomRoom")]
public class CustomRoom : RoomBase
{
    public RoomType RoomType;

    [Header("Room Templates")]
    public GameObject[] BossRoomTemplates;
    public GameObject[] EnemyRoomTemplates;
    public GameObject[] EliteEnemyRoomTemplates;
    public GameObject[] ShopRoomTemplates;
    public GameObject[] TreasureRoomTemplates;
    public GameObject[] BirthRoomTemplates;
    public GameObject[] TeleportRoomTemplates;
    public GameObject[] SecretRoomTemplates;
    public GameObject[] BasicRoomTemplates;

    public override string GetDisplayName() => RoomType.ToString();

    // ������ʱ��ȷ������ģ�����鶼��Ϊ null
    private void OnEnable()
    { // ����Ĭ��ֵ
        if (RoomType == RoomType.Unassigned)
        {
            Debug.LogWarning($"δ��ʼ���ķ������ͣ��Զ�����ΪĬ��ֵ");
            RoomType = RoomType.BasicRoom; // �� ���_����
        }
        if (BossRoomTemplates == null) BossRoomTemplates = new GameObject[0];
        if (EnemyRoomTemplates == null) EnemyRoomTemplates = new GameObject[0];
        if (EliteEnemyRoomTemplates == null) EliteEnemyRoomTemplates = new GameObject[0];
        if (ShopRoomTemplates == null) ShopRoomTemplates = new GameObject[0];
        if (TreasureRoomTemplates == null) TreasureRoomTemplates = new GameObject[0];
        if (BirthRoomTemplates == null) BirthRoomTemplates = new GameObject[0];
        if (TeleportRoomTemplates == null) TeleportRoomTemplates = new GameObject[0];
        if (SecretRoomTemplates == null) SecretRoomTemplates = new GameObject[0];
        if (BasicRoomTemplates == null) BasicRoomTemplates = new GameObject[0];
    }

    public override List<GameObject> GetRoomTemplates()
    {
        GameObject[] templates = null;
        switch (RoomType)
        {
            case RoomType.BossRoom:
                templates = BossRoomTemplates;
                break;
            case RoomType.EnemyRoom:
                templates = EnemyRoomTemplates;
                break;
            case RoomType.EliteEnemyRoom:
                templates = EliteEnemyRoomTemplates;
                break;
            case RoomType.ShopRoom:
                templates = ShopRoomTemplates;
                break;
            case RoomType.TreasureRoom:
                templates = TreasureRoomTemplates;
                break;
            case RoomType.BirthRoom:
                templates = BirthRoomTemplates;
                break;
            case RoomType.TeleportRoom:
                templates = TeleportRoomTemplates;
                break;
            case RoomType.SecretRoom:
                templates = SecretRoomTemplates;
                break;
            default:
                templates = BasicRoomTemplates;
                break;
        }

        if (templates == null)
            templates = new GameObject[0];

        return new List<GameObject>(templates);
    }
}