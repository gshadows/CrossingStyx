using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SillyHuman : MyBehaviour {
    [Range(0f, 120f)] public float startWalkingDelaySec = 2f; // Walking delay at start of game.
    [Range(0f, 120f)] public float restingDelaySec = 0.5f; // Walking delay after while resting between movements.
    [Range(0f, 1f)] public float walkingSpeed = 0.25f;
    [Range(0f, 2f)] public float walkingLimit = 1f; // Position range [-1..+1] corresponds to X-coordinate [-walkingLimit..+wakingLimit].

    public AudioClip scream;

    private float position = 0;         // Our current position on boat in [-1..+1] range.
    private float targetPosition = 0;   // Target position on boat (where we go).
    private bool missionFailed = false; // True if boat is capsizing/sinking (player failed).
    private float nextTimeToMove;       // Delay before next movement.

    public VoidEventSO capsizingEvent;
    public VoidEventSO sinkingEvent;


    void Start() {
        capsizingEvent.addListener(onMissionFailed);
        sinkingEvent.addListener(onMissionFailed);
        onRestart();
    }

    void onRestart() {
        nextTimeToMove = Time.time + startWalkingDelaySec;
        position = targetPosition = 0;
        missionFailed = false;
    }

    void FixedUpdate() {
        if (missionFailed || !GameControl.instance.isPlaying()) {
            // Nothing here.
        } else if (Time.time >= nextTimeToMove) {
            // Make some problems for player :)
            proceedMovements();
        } else {
            // Resting: waiting some time before walk.
        }
    }


    private void proceedMovements() {
        if (Mathf.Abs(targetPosition - position) < walkingSpeed * Time.fixedDeltaTime) {
            // We're done walking. Prepare to next move.
            nextTimeToMove = Time.time + restingDelaySec;
            targetPosition = Random.value * 2f - 1f;
        } else {
            // Walking now.
            float deltaX = Mathf.Sign(targetPosition - position) * walkingSpeed * Time.fixedDeltaTime;
            position += deltaX;
            transform.Translate(deltaX * walkingLimit, 0, 0); // Position is [-1..+1], so walking limit is actually a scale position to X-coordinate.
        }
    }


    private void onMissionFailed() {
        missionFailed = true;
        StartCoroutine("delayedScream");
    }


    IEnumerator delayedScream() {
        yield return new WaitForSeconds(1f + Random.value * 2f);
        sound(scream);
    }


    public float getPosition() {
        return position;
    }
}
