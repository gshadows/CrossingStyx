using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnBoat : MonoBehaviour
{
    public float mouseSensitivity = 2;
    public float upDownLookRange = 90;

    float curAngleLR = 0;
    float curAngleUD = 0;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Rotation.
        float rotLeftRight = Input.GetAxis("Mouse X") * mouseSensitivity;
        curAngleLR += rotLeftRight;

        float rotUpDown = Input.GetAxis("Mouse Y") * mouseSensitivity;
        curAngleUD = Mathf.Clamp(curAngleUD - rotUpDown, -upDownLookRange, upDownLookRange);

        //transform.Rotate(rotUpDown, rotLeftRight, 0);
        transform.localRotation = Quaternion.Euler(curAngleUD, curAngleLR, 0);
    }
}
