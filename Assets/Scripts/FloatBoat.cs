using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FloatBoat : MyBehaviour {
    public enum State { FLOATING, CAPSIZING, SINKING };

    public Waves waves;
    [Range(-5f, +5f)] public float waterLine = 0f;
    [Range(0.01f, 10f)] public float boatSpeedMPS = 0.25f;
    [Range(-0.01f, -2f)] public float sinkingSpeed = -0.1f;
    [Range(0.01f, 45f)] public float rollChangeSpeed = 30f;
    [Range(0.01f, 45f)] public float rollSpeedCapsize = 5f;
    [Range(0f, 30f)] public float wavesMaxRoll = 15f;
    [Range(0f, 90f)] public float capsizeRoll = 45f;
    [Range(0f, 90f)] public float sinkRoll = 90f;
    [Range(0f, 90f)] public float humanMaxRoll = 30f;

    public PlayerOnBoat player;
    public SillyHuman[] sillyHumans;

    public AudioClip capsizing;
    public AudioClip sinking;

    [ReadOnly] public State state;
    [ReadOnly] public float roll; // Current boat roll.
    [ReadOnly] public float initialBaotPosZ;

    private float startCapsizeRoll;
    private float startCapsizeTime;
    private AudioSource waterAudioSource;

    public VoidEventSO capsizingEvent;
    public VoidEventSO sinkingEvent;


    void Start() {
        waterAudioSource = GetComponent<AudioSource>();
        initialBaotPosZ = transform.position.z;
        onRestart();
    }

    void onRestart() {
        startCapsizeRoll = startCapsizeTime = 0;
        state = State.FLOATING;
        roll = 0f;
        Vector3 position = new Vector3(transform.position.x, transform.position.y, initialBaotPosZ);
        transform.SetPositionAndRotation(position, transform.rotation);
    }


    void FixedUpdate() {
        if (!GameControl.instance.isPlaying()) {
            return;
        }

        float deltaZ = 0f; // Move forward.
        float deltaY = 0f; // Skinking.
        float rollWaveDelta = 0f;

        switch (state) {
            case State.FLOATING:
                if (Mathf.Abs(roll) > capsizeRoll) {
                    Debug.Log("CAPSIZING!!!");
                    state = State.CAPSIZING;
                    startCapsizeRoll = roll;
                    startCapsizeTime = Time.time;
                    capsizingEvent.raise();
                    delayedSound(capsizing, 1f + Random.value);
                    break;
                }
                rollWaveDelta = wavesMaxRoll * Mathf.Sin(Time.fixedTime); // Waves effect - temporary roll delta.
                deltaZ = boatSpeedMPS * Time.fixedDeltaTime; // Move forward.
                // Changing roll by silly pasengers.
                float targetRoll = calculateTargetRoll();
                float dr = rollChangeSpeed * Time.fixedDeltaTime;
                float diff = targetRoll - roll;
                if (Mathf.Abs(diff) > dr) {
                    roll += dr * Mathf.Sign(diff);
                } else {
                    roll = targetRoll;
                }
                break;

            case State.CAPSIZING:
                // Slowly rotate to the side before going down.
                if (Mathf.Abs(roll) > sinkRoll) {
                    Debug.Log("SINKING!!!");
                    state = State.SINKING;
                    sinkingEvent.raise();
                    delayedSound(sinking, 1f + Random.value);
                    break;
                }
                // Capsizing boat by uncontrollable increasing roll.
                float dt = Time.time - startCapsizeTime;
                roll = startCapsizeRoll + Mathf.Sign(startCapsizeRoll) * rollSpeedCapsize * dt;
                // Reduce speed to zero while capsizing.
                float fullCapsizeTime = (sinkRoll - capsizeRoll) / rollSpeedCapsize;
                deltaZ = boatSpeedMPS * (1f - dt / fullCapsizeTime) * Time.fixedDeltaTime;
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
        float finalRoll = (roll + rollWaveDelta) * -1; // Because rolling on left size is positive Z rotation, whilst moving left is negative X movement.
        Quaternion rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, finalRoll);
        Vector3 position = new Vector3(transform.position.x, newY + deltaY, transform.position.z + deltaZ);
        transform.SetPositionAndRotation(position, rotation);
    }


    private float calculateTargetRoll() {
        float targetRoll = player.getPosition() * humanMaxRoll;
        foreach (SillyHuman human in sillyHumans) {
            targetRoll += human.getPosition() * humanMaxRoll;
        }
        return targetRoll;
    }


    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Finish") {
            switch (state) {
                case State.FLOATING:
                    GameControl.instance.onBoatArrives(true);
                    break;
                case State.CAPSIZING:
                    GameControl.instance.onBoatArrives(false);
                    break;
            }
        }
    }


    public void switchWaterSound(bool enable) {
        waterAudioSource.loop = enable;
        if (enable) {
            waterAudioSource.Play();
        } else {
            waterAudioSource.Stop();
        }
    }
}
