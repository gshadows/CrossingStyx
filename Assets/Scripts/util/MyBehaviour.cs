using System;
using System.Collections;
using UnityEngine;

public class MyBehaviour : MonoBehaviour {
    protected void showMouse(bool show) {
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = show;
    }


    protected void sound(AudioClip audioClip, float volume = 1f) {
        if (audioClip != null) {
            AudioSource.PlayClipAtPoint(audioClip, transform.position, volume);
        }
    }


    protected void soundGlobal(AudioClip audioClip, float volume = 1f) {
        if (audioClip != null) {
            AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position, volume);
        }
    }


    private struct DelayedSound {
        public AudioClip audioClip;
        public float delay, volume;
        public Vector3 position;
    }
    protected void delayedSound(AudioClip audioClip, float delay, float volume = 1f) {
        if (audioClip != null) {
            DelayedSound ds = new DelayedSound();
            ds.audioClip = audioClip;
            ds.delay = delay;
            ds.volume = volume;
            ds.position = transform.position;
            StartCoroutine("delayedSoundProc", ds);
        }
    }
    protected void delayedSoundGlobal(AudioClip audioClip, float delay, float volume = 1f) {
        if (audioClip != null) {
            DelayedSound ds = new DelayedSound();
            ds.audioClip = audioClip;
            ds.delay = delay;
            ds.volume = volume;
            ds.position = Camera.main.transform.position;
            StartCoroutine("delayedSoundProc", ds);
        }
    }
    IEnumerator delayedSoundProc(DelayedSound ds) {
        yield return new WaitForSeconds(ds.delay);
        AudioSource.PlayClipAtPoint(ds.audioClip, ds.position, ds.volume);
    }


    protected void globalBroadcast (string method, object param = null) {
        var everyone = GameObject.FindObjectsOfType<GameObject>(true);
        foreach (var obj in everyone) {
            if (obj.GetComponent<MyBehaviour>() != null) {
                obj.SendMessage(method, param, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
