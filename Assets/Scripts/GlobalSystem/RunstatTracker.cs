using System;
using UnityEngine;

public class RunStatTracker : MonoBehaviour {
    public static RunStatTracker Instance { get; private set; }

    public float TotalRunTimer { get; private set; }
    public float CurrentRunTimer { get; private set; }

    public int TotalCardsUsed { get; private set; }
    public int CurrentCardsUsed { get; private set; }

    public int TotalEnemiesKilled { get; private set; }
    public int CurrentEnemiesKilled { get; private set; }

    public int TotalDamageAmount { get; private set; }
    public int CurrentDamageAmount { get; private set; }

    private bool isTracking = false;

    private void Awake() {

        if (Instance != null && Instance != this) {

            Destroy(gameObject);
            return;

        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ResetTotalStats();
        ResetCurrentStats();

    }

    public void StartTracking() {

        isTracking = true;

    }

    public void StopTracking() {

        isTracking = false;

    }


    void Update() {

        if (isTracking)
            CurrentRunTimer += Time.deltaTime;

    }

    public void ResetCurrentStats() {

        CurrentRunTimer = 0f;
        CurrentCardsUsed = 0;
        CurrentEnemiesKilled = 0;
        CurrentDamageAmount = 0;

    }

    public void ResetTotalStats() {

        CurrentRunTimer = 0f;
        TotalCardsUsed = 0;
        TotalEnemiesKilled = 0;
        TotalDamageAmount = 0;

    }

    public void ApplyTotalStats() {

        TotalRunTimer += CurrentRunTimer;
        TotalCardsUsed += CurrentCardsUsed;
        TotalEnemiesKilled += CurrentEnemiesKilled;
        TotalDamageAmount += CurrentDamageAmount;

    }

    public void RecordCardUsed() => CurrentCardsUsed++;
    public void RecordEnemyKilled() => CurrentEnemiesKilled++;
    public void RecordDamage(int amount) => CurrentDamageAmount += amount;

    public string IntToTotalTime() {

        TimeSpan time = TimeSpan.FromSeconds(TotalRunTimer);

        return string.Format("通关总用时：{0:D2}:{1:D2}", time.Minutes, time.Seconds);

    }

    public string HandleCompleteLevelText() {

        string level;

        int totalMinutes = Mathf.FloorToInt(TotalRunTimer / 60f);

        if (totalMinutes <= 15) {

            level = "SS";

        }
        else if (totalMinutes <= 18) {

            level = "S";

        }
        else if (totalMinutes <= 21) {

            level = "A";

        }
        else if (totalMinutes <= 24) {

            level = "B";

        }
        else if (totalMinutes <= 27) {

            level = "C";

        }
        else {

            level = "D";

        }

        return $"评级：{level}";

    }

}
