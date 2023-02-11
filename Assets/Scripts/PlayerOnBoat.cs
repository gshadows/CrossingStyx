using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnBoat : MyBehaviour {
    [Range(0f, 1f)] public float walkingSpeed = 0.25f;
    [Range(0f, 2f)] public float walkingLimit = 1f; // Position range [-1..+1] corresponds to X-coordinate [-walkingLimit..+wakingLimit].

    private float position = 0f;
    private float startX;
    private bool missionFailed = false;

    public VoidEventSO capsizingEvent;
    public VoidEventSO sinkingEvent;


    private void Start() {
        startX = transform.localPosition.x;
        capsizingEvent.addListener(onMissionFailed);
        sinkingEvent.addListener(onMissionFailed);
        onRestart();
    }

    void onRestart() {
        position = 0f;
        missionFailed = false;
    }

    void Update()
    {
        if (!GameControl.instance.isPlaying()) {
            return;
        }
        if (!missionFailed) {
            walking();
        }
    }


    private void walking() {
        float deltaX = Input.GetAxis("Horizontal") * walkingSpeed * Time.deltaTime;
        position = Mathf.Clamp(position + deltaX, -1f, +1f);
        Vector3 pos = new Vector3(startX + position * walkingLimit, transform.localPosition.y, transform.localPosition.z);
        transform.localPosition = pos;
    }


    private void onMissionFailed() {
        missionFailed = true;
    }


    public float getPosition() {
        return position;
    }
}
