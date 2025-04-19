using UnityEngine;

public class FloorDetector : MonoBehaviour {

    [SerializeField] public bool IsFloorExisting;
    [SerializeField] public Transform Found;
    void Start() {

        IsFloorExisting = true;
        detectFloor();

    }

    private void detectFloor() {

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.001f);

        CustomLogger.LogWarning($"{cols.Length} floor found!");

        if (cols.Length == 0) {

            IsFloorExisting = false;

        }
        else {

            Found = cols[0].transform;

        }

    }

}
