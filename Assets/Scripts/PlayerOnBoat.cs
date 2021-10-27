using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnBoat : MonoBehaviour
{
    [Range(0f, 1f)] public float walkingSpeed = 0.25f;
    [Range(0f, 2f)] public float walkingLimit = 1f; // Position range [-1..+1] corresponds to X-coordinate [-walkingLimit..+wakingLimit].

    private float position = 0f;
    private float startX;
    private bool missionFailed = false;
    private FloatBoat boat;


    private void Start() {
        startX = transform.localPosition.x;
        boat = GameObject.FindObjectOfType<FloatBoat>();
        boat.onCapsize += onMissionFailed;
        boat.onSink += onMissionFailed;
    }

    void Update()
    {
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
