using System;
using UnityEngine;

public class MyBehaviour : MonoBehaviour {
    public void showMouse(bool show) {
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = show;
    }


    protected void sound(AudioClip audioClip, float volume = 2f) {
        if (audioClip != null) {
            AudioSource.PlayClipAtPoint(audioClip, transform.position, volume);
        }
    }
}
