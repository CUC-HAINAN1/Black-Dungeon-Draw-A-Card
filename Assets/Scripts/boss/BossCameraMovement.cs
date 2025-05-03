using UnityEngine;
using System.Collections;

public class BossCameraMovement : MonoBehaviour {

    [Header("配置")]
    public GameObject CameraController;
    public Vector3 cameraOffset = new Vector3(-1, 1.25f, 0);
    public float freezeTime = 0.3f;
    public float moveTime = 0.5f;
    public float holdTime = 1.5f;

    private Vector3 origPos;

    // 在 Boss 刷新时调用
    public void PlayCinematic(Transform bossTransform) {

        StartCoroutine(CinematicRoutine(bossTransform));

    }

    private IEnumerator CinematicRoutine(Transform bossTransform) {

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(freezeTime);

        // 4. 平滑移动到 Boss
        Vector3 targetPos = bossTransform.position + cameraOffset;
        origPos = CameraController.transform.position;

        float t = 0f;
        while (t < moveTime) {

            t += Time.unscaledDeltaTime;

            float alpha = Mathf.Clamp01(t / moveTime);
            CameraController.transform.position = Vector3.Lerp(origPos, targetPos, alpha);

            yield return null;

        }

        // 5. 停留一会
        yield return new WaitForSecondsRealtime(holdTime);

        Time.timeScale = 1f;

        t = 0f;
        while (t < moveTime) {

            t += Time.unscaledDeltaTime;

            float alpha = Mathf.Clamp01(t / moveTime);
            CameraController.transform.position = Vector3.Lerp(targetPos, origPos, alpha);

            yield return null;

        }

    }
}
