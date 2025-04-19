using Edgar.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomPostProcessing : DungeonGeneratorPostProcessingComponentGrid2D
{
    private List<RoomInstanceGrid2D> roomInstances;

    public override void Run(DungeonGeneratorLevelGrid2D level)
    {
        if (level == null || level.RoomInstances == null)
        {
            CustomLogger.LogError("Invalid level data");
            return;
        }

        roomInstances = level.RoomInstances;

        foreach (var roomInstance in roomInstances)
        {
            if (roomInstance?.RoomTemplateInstance == null)
            {
                CustomLogger.LogWarning("Skipping invalid room instance");
                continue;
            }

            ProcessFloor(roomInstance.RoomTemplateInstance);
        }
    }

    private void ProcessFloor(GameObject roomTemplate)
    {
        var tilemaps = RoomTemplateUtilsGrid2D.GetTilemaps(roomTemplate);
        var floorTilemap = tilemaps.FirstOrDefault(x => x.name == "Floor");

        if (floorTilemap == null)
        {
            CustomLogger.LogError($"Floor tilemap not found in room: {roomTemplate.name}");
            return;
        }

        var floor = floorTilemap.gameObject;
        ConfigureFloorCollider(floor);
    }

    private void ConfigureFloorCollider(GameObject floor)
    {
        var rb = floor.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = floor.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static; // ȷ������BodyType
            CustomLogger.LogWarning($"�Զ�Ϊ {floor.name} ������Rigidbody2D");
        }

        // ȷ����Ҫ���������
        var rigidbody = floor.GetComponent<Rigidbody2D>() ?? floor.AddComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Static;


        /* var compositeCollider = floor.GetComponent<CompositeCollider2D>() ?? floor.AddComponent<CompositeCollider2D>();
        compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;
        compositeCollider.isTrigger = true;
        compositeCollider.generationType = CompositeCollider2D.GenerationType.Synchronous; */
    }

    public List<RoomInstanceGrid2D> GetRoomInstances() => roomInstances;
}
