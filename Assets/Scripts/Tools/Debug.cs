using UnityEngine;

public class CustomLogger : MonoBehaviour {

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void Log(string message) {

        Debug.Log(message);

    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogWarning(string message) {

        Debug.LogWarning(message);

    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void LogError(string message) {

        Debug.LogError(message);

    }

}
