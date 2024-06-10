using UnityEngine;
using System.Diagnostics;

public class DebugMode : MonoBehaviour {
    static bool debugMode = false;
    private GameObject player;
    private PlayerState playerStateComponent;

    private Fatigue fatigueComponent;

    private static Stopwatch timer = new();
    private GameObject checkpoint;
    private checkpoint checkpointComponent;


    private int deathCount = 0;
    private bool isDeathHandled = false;
    private int minutes = -1;
    private bool isMinuteLeft = true;

    private const string DeathCountKey = "DeathCount";
    private const string Minutes = "Minutes";

    void Start() {
        timer.Start();
        player = GameObject.Find("Player");
        playerStateComponent = player.GetComponent<PlayerState>();

        fatigueComponent = player.GetComponent<Fatigue>();
        deathCount = PlayerPrefs.GetInt(DeathCountKey, 0);
        minutes = PlayerPrefs.GetInt(Minutes, 0);
        checkpoint = GameObject.Find("Checkpoint");
        checkpointComponent = checkpoint.GetComponent<checkpoint>();

    }
    // Update is called once per frame
    void Update() {

        if (Input.GetKeyDown(KeyCode.F12)) {
            ClickListener();
        }

        if (playerStateComponent.state == PlayerState.State.death && !isDeathHandled) {
            deathCount++;
            isDeathHandled = true;

            PlayerPrefs.SetInt(DeathCountKey, deathCount);
            PlayerPrefs.Save();
        }
        else if (playerStateComponent.state != PlayerState.State.death) {
            isDeathHandled = false;
        }


        if (timer.Elapsed.Seconds > 0 && timer.Elapsed.Seconds <= 59) {
            isMinuteLeft = false;

        }
        else if (timer.Elapsed.Seconds == 0 && !isMinuteLeft) {
            isMinuteLeft = true;
            minutes++;
            PlayerPrefs.SetInt(Minutes, minutes);
            PlayerPrefs.Save();
        }
    }

    void ClickListener() {
        debugMode = !debugMode;
        if (debugMode) {
            UnityEngine.Debug.Log("Debug mode activated!");
        }
        else {
            UnityEngine.Debug.Log("Debug mode deactivated!");
        }
    }

    void OnGUI() {
        if (debugMode) {
            DebugModeActivated();
        }

    }

    void DebugModeActivated() {
        PrintOnScreen(0, 0, "Debug information");
        PrintOnScreen(0, 10, "Player Position: " +
            player.transform.position.ToString());
        PrintOnScreen(0, 20, "Player State: " +
            playerStateComponent.state.ToString());
        PrintOnScreen(0, 30, "Player Fatigue: " +
            fatigueComponent.fatigue.ToString());
        PrintOnScreen(0, 40, "Deaths: " + deathCount.ToString());
        PrintOnScreen(0, 50, "Timer: " + timer.Elapsed.Seconds.ToString());
        PrintOnScreen(0, 60, "Minutes spent: " + minutes.ToString());
        PrintOnScreen(0, 70, "Current checkpoint: " + checkpointComponent.index.ToString());
    }

    void PrintOnScreen(int x, int y, string str) {
        GUI.Label(new Rect(x, y, Screen.width, Screen.height), str);
    }
}
