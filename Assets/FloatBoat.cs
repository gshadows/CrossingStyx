using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatBoat : MonoBehaviour
{
    public Waves waves;
    public float waterLine = 0f;

    void FixedUpdate() {
        float? waterLevel = waves.getHeight(transform.position);
        if (waterLevel != null) {
            float newY = waterLine + (float)waterLevel;
            transform.SetPositionAndRotation(new Vector3(transform.position.x, newY, transform.position.z), transform.rotation);
        }
    }
}
