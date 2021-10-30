using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaronScript : MyBehaviour {
    public int secondsToSayBalance = 3;
    public AudioClip keepBalance;
    public AudioClip idiots;
    public AudioClip woosh;

    private FloatBoat boat;


    void Start()
    {
        boat = GameObject.FindObjectOfType<FloatBoat>();
        boat.onCapsize += onCapsize;
        boat.onSink += onSink;
        onRestart();
    }


    void onRestart() {
        gameObject.SetActive(true);
        StartCoroutine("sayKeepBalance");
    }


    IEnumerator sayKeepBalance() {
        yield return new WaitForSeconds(secondsToSayBalance);
        sound(keepBalance);
    }


    private void onCapsize() {
        sound(idiots);
    }


    private void onSink() {
        gameObject.SetActive(false);
        sound(woosh);
        // TODO: Instantiate particles.
    }
}
