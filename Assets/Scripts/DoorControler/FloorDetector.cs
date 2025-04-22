using UnityEngine;
using System.Collections;

public class FloorDetector : MonoBehaviour {

    [SerializeField] public bool IsFloorExisting;
    [SerializeField] public Transform Found;
    void Start() {

        IsFloorExisting = true;

        StartCoroutine(detectFloor());

    }

    private IEnumerator detectFloor() {

        yield return new WaitForSeconds(0.5f);

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
