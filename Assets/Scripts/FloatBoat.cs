using System;
using System.Collections;
using UnityEngine;

public class FloatBoat : MonoBehaviour
{
    public delegate void CapsizeListener();
    public delegate void SinkListener();
    public event CapsizeListener onCapsize;
    public event SinkListener onSink;

    public enum State { FLOATING, CAPSIZING, SINKING };

    public Waves waves;
    [Range(-5f, +5f)] public float waterLine = 0f;
    [Range(0.01f, 10f)] public float boatSpeedMPS = 0.25f;
    [Range(-0.01f, -2f)] public float sinkingSpeed = -0.1f;
    [Range(0.01f, 2f)] public float rollChangeSpeed = 0.1f;
    [Range(0f, 30f)] public float wavesMaxRoll = 15f;
    [Range(0f, 90f)] public float capsizeRoll = 45f;
    [Range(0f, 90f)] public float sinkiRoll = 90f;
    [Range(0f, 90f)] public float humanMaxRoll = 30f;

    public PlayerOnBoat player;
    public SillyHuman[] sillyHumans;

    [ReadOnly] public State state = State.FLOATING;
    [ReadOnly] public float roll = 0f; // Current boat roll.

    private float startCapsizeRoll;

    void FixedUpdate() {
        float deltaZ = 0f; // Move forward.
        float deltaY = 0f; // Skinking.
        float rollWaveDelta = 0f;

        switch (state) {
            case State.FLOATING:
                if (Mathf.Abs(roll) > capsizeRoll) {
                    Debug.Log("CAPSIZING!!!");
                    state = State.CAPSIZING;
                    startCapsizeRoll = roll;
                    onCapsize();
                    break;
                }
                rollWaveDelta = wavesMaxRoll * Mathf.Sin(Time.fixedTime); // Waves effect - temporary roll delta.
                deltaZ = boatSpeedMPS * Time.fixedDeltaTime; // Move forward.
                // Changing roll by silly pasengers.
                float targetRoll = calculateTargetRoll();
                float dr = rollChangeSpeed * Time.fixedDeltaTime;
                if (Mathf.Abs(targetRoll - roll) > dr) {
                    roll += dr * Mathf.Sign(targetRoll);
                } else {
                    roll = targetRoll;
                }
                break;

            case State.CAPSIZING:
                // Slowly rotate to the side before going down.
                if (Mathf.Abs(roll) > sinkiRoll) {
                    Debug.Log("SINKING!!!");
                    state = State.SINKING;
                    onSink();
                    break;
                }
                roll += Mathf.Lerp(startCapsizeRoll, 90f * Mathf.Sign(startCapsizeRoll), Time.fixedDeltaTime); // Roll: 45� -> 90�.
                deltaZ = Mathf.Lerp(boatSpeedMPS, 0, Time.fixedDeltaTime) * Time.fixedDeltaTime; // Reducing speed to zero.
                break;

            case State.SINKING:
                // Just go down vertically.
                deltaY = sinkingSpeed * Time.fixedDeltaTime;
                break;

            default:
                // Something goes wrong.
                Debug.LogErrorFormat("Unsupported state: {0}", state);
                state = State.FLOATING;
                return;
        }

        // Calculate water Y coordinate taking wave height into account.
        float newY;
        float? waterLevel = waves.getHeight(transform.position);
        if (waterLevel != null) {
            newY = waterLine + (float)waterLevel;
        } else {
            newY = transform.position.y;
        }

        // Apply new position and rotation.
        Quaternion rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, roll + rollWaveDelta);
        Vector3 position = new Vector3(transform.position.x, newY + deltaY, transform.position.z + deltaZ);
        transform.SetPositionAndRotation(position, rotation);
    }


    private float calculateTargetRoll() {
        float roll = player.getPosition() * humanMaxRoll;
        string dbg = "" + Mathf.Round(roll) + "� vs ";
        foreach (SillyHuman human in sillyHumans) {
            roll += human.getPosition() * humanMaxRoll;
            dbg += Mathf.Round(human.getPosition() * humanMaxRoll) + "�, ";
        }
        dbg += "-> " + Mathf.Round(roll);
        Debug.Log(dbg);
        return roll;
    }
}
