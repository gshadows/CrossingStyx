using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MyBehaviour {
    public float mouseSensitivity = 2;
    public float upDownLookRange = 90;

    private float curAngleLR = 0f;
    private float curAngleUD = 0f;


    void Update()
    {
        if (!GameControl.instance.isMenu()) {
            mouseLook();
        }
    }


    void onRestart() {
        curAngleLR = curAngleUD = 0;
    }


    private void mouseLook() {
        // Rotation.
        float rotLeftRight = Input.GetAxis("Mouse X") * mouseSensitivity;
        curAngleLR += rotLeftRight;

        float rotUpDown = Input.GetAxis("Mouse Y") * mouseSensitivity;
        curAngleUD = Mathf.Clamp(curAngleUD - rotUpDown, -upDownLookRange, upDownLookRange);

        transform.localRotation = Quaternion.Euler(curAngleUD, curAngleLR, 0);
    }
}
