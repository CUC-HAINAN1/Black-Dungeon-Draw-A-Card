using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;

    [Header("跟随参数")]
    public float followSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    void LateUpdate()
    {
        if (target != null) {

            Vector3 targetPosition = target.position + offset;

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

        }

    }
}
