using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaronScript : MyBehaviour {
    public int secondsToSayBalance = 3;
    public ParticleSystem teleportParticles;
    public AudioClip keepBalance;
    public AudioClip idiots;
    public AudioClip teleport;

    private FloatBoat boat;


    void Start()
    {
        boat = GameObject.FindObjectOfType<FloatBoat>();
        boat.onCapsize += onCapsize;
        boat.onSink += onSink;
        onRestart();
    }


    public void onRestart() {
        gameObject.SetActive(true);
        Debug.Log("Hello from Haron!");
        StartCoroutine("sayKeepBalance");
    }


    IEnumerator sayKeepBalance() {
        yield return null;
        yield return new WaitUntil(() => (GameControl.instance.gameStage == GameControl.GameStage.PLAY));
        yield return new WaitForSeconds(secondsToSayBalance);
        soundGlobal(keepBalance);
        onSink();
    }


    private void onCapsize() {
        soundGlobal(idiots);
    }


    private void onSink() {
        gameObject.SetActive(false);
        soundGlobal(teleport);

        //GameObject splash = Instantiate(teleportParticles, transform.position, transform.rotation);
        //splash.transform.SetParent(transform.parent, true);
        //Destroy(splash, 5f);
        teleportParticles.Play();
    }
}
