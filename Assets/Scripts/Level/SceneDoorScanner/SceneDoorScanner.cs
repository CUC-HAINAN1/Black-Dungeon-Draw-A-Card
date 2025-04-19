using UnityEngine;

public class SceneDoorScanner : MonoBehaviour
{
    public static SceneDoorScanner Instance;

    [Header("ɨ������")]
    [SerializeField] private float initDelay = 0.5f;
    private DoorStateController[] allDoors;

    void Awake()
    {
        Instance = this;
        Invoke("InitializeDoors", initDelay);
    }

    void InitializeDoors()
    {
        allDoors = FindObjectsOfType<DoorStateController>();
        foreach (var door in allDoors)
        {
            door.InitDoorState();
        }
    }

    public void SetAllDoorsOpenState(bool open)
    {

        if (allDoors == null)
            return;

        foreach (var door in allDoors) {
            if (door == null)
                continue;

            if (open)
                door.OpenDoor();
            else
                door.CloseDoor();

            door.UpdateNavigationGraph(open ? door.upDoor : door.downDoor);
        }
    }

    public void ToggleRoomDoors(RoomManager room, bool open)
    {
        foreach (var door in room.doors)
        {
            if (door == null) continue;

            if (open) door.OpenDoor();
            else door.CloseDoor();

            door.UpdateNavigationGraph(open ? door.upDoor : door.downDoor);
        }
    }
}
