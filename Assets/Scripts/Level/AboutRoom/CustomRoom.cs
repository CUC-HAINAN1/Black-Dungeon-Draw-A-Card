using Edgar.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomRoom : RoomBase
{
    public RoomType RoomType;

    public override string GetDisplayName() => RoomType.ToString();

    public override List<GameObject> GetRoomTemplates()
    {
        return null;
    }
}
