using System.Collections.Generic;
using UnityEngine;

public class GlobalRoomSystem : MonoBehaviour
{
    public static List<RoomManager> activeRooms = new List<RoomManager>();


    public static void UpdateConnectedRooms(Vector2 combatPosition)
    {
        foreach (var room in activeRooms)
        {
            if (IsRoomInRange(room.transform.position, combatPosition))
            {
                room.CloseAllDoors();
            }
        }
    }
    private static bool IsRoomInRange(Vector2 pos1, Vector2 pos2)
    {
        // ��Edgar����ת��Ϊ��Ϸ������
        float scaledDistance = Vector2.Distance(pos1 / 1000f, pos2 / 1000f);
        return Vector2.Distance(pos1, pos2) < 12f;
    }
}
