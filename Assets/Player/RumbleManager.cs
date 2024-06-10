using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class RumbleManager : MonoBehaviour {
    [Header("Gamepad properties")]
    [SerializeField] public float dashLowFrequency = 0.7f;
    [SerializeField] public float dashHighFrequency = 0.8f;
    [SerializeField] private float dashRumbleDuration = 0.4f;
    [Header("Fatigue rumble")]
    [SerializeField] private float fatigueLowFrequency = 0.2f;
    [SerializeField] private float fatigueHighFrequency = 0.35f;

    public bool isRumbling = false;

    private Gamepad pad;

    void OnDisable() {
        StopRumble();
    }

    void Update() {
        if (!isRumbling) {
            StopRumble();
        }
    }

    public void DashRumblePulse() {
        pad = Gamepad.current;

        if (pad != null) {
            isRumbling = true;
            pad.SetMotorSpeeds(dashLowFrequency, dashHighFrequency);
            StartCoroutine(StopDashRumble(pad));
        }
    }

    private IEnumerator StopDashRumble(Gamepad pad) {
        float elapsedTime = 0f;

        while (elapsedTime <= dashRumbleDuration) {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        pad.SetMotorSpeeds(0f, 0f);
        isRumbling = false;
    }

    public void startFatigueRumble() {
        pad = Gamepad.current;

        if (pad != null) {
            isRumbling = true;
            pad.SetMotorSpeeds(fatigueLowFrequency, fatigueHighFrequency);
        }

    }

    public void endFatigueRumble() {
        pad = Gamepad.current;

        StopRumble();
    }

    void OnApplicationQuit() {
        pad = Gamepad.current;

        StopRumble();
    }

    public void StopRumble() {
        if (pad != null) {
            isRumbling = false;
            pad.SetMotorSpeeds(0f, 0f);
        }
    }
}
