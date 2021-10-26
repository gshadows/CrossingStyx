using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaronScript : MonoBehaviour
{
    public FloatBoat boat;


    void Start()
    {
        boat.onCapsize += onCapsize;
        boat.onSink += onSink;
    }

    private void onCapsize() {
        // Idiots!!!
    }


    private void onSink() {
        Destroy(gameObject);
        // TODO: Woosh sound.
        // TODO: Instantiate particles.
    }


    void Update()
    {
        
    }
}
