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

    [ReadOnly] public State state = State.FLOATING;
    [ReadOnly] public float roll = 0f; // Current boat roll.

    private float startCapsizeRoll;
    private float targetRoll = 0f; // Boat roll induced by silly passengers.

    void FixedUpdate() {
        float deltaZ = 0f; // Move forward.
        float deltaY = 0f; // Skinking.
        float rollWaveDelta = 0f;

        switch (state) {
            case State.FLOATING:
                if (Mathf.Abs(roll) > 45f) {
                    Debug.Log("CAPSIZING!!!");
                    state = State.CAPSIZING;
                    startCapsizeRoll = roll;
                    onCapsize();
                    break;
                }
                rollWaveDelta = wavesMaxRoll * Mathf.Sin(Time.fixedTime); // Waves effect - temporary roll delta.
                deltaZ = boatSpeedMPS * Time.fixedDeltaTime; // Move forward.
                // Changing roll by silly pasengers.
                float dr = rollChangeSpeed * Time.fixedDeltaTime;
                if (Mathf.Abs(targetRoll - roll) > dr) {
                    roll += dr * Mathf.Sign(targetRoll);
                } else {
                    roll = targetRoll;
                }
                break;
            case State.CAPSIZING:
                if (Mathf.Abs(roll) > 90f) {
                    Debug.Log("SINKING!!!");
                    state = State.SINKING;
                    onSink();
                    break;
                }
                roll += Mathf.Lerp(startCapsizeRoll, 90f * Mathf.Sign(startCapsizeRoll), Time.fixedDeltaTime); // Roll: 45° -> 90°.
                deltaZ = Mathf.Lerp(boatSpeedMPS, 0, Time.fixedDeltaTime) * Time.fixedDeltaTime; // Reducing speed to zero.
                break;
            case State.SINKING:
                deltaY = sinkingSpeed * Time.fixedDeltaTime;
                break;
            default:
                Debug.LogErrorFormat("Unsupported state: {0}", state);
                state = State.FLOATING;
                return;
        }

        float newY;
        float? waterLevel = waves.getHeight(transform.position);
        if (waterLevel != null) {
            newY = waterLine + (float)waterLevel;
        } else {
            newY = transform.position.y;
        }

        Quaternion rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, roll + rollWaveDelta);
        transform.SetPositionAndRotation(new Vector3(transform.position.x, newY + deltaY, transform.position.z + deltaZ), rotation);
    }


    public void setTargetRoll (float newTargetRoll) {
        targetRoll = newTargetRoll;
    }
}
