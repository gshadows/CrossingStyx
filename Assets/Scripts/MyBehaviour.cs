using System;
using UnityEngine;

public class MyBehaviour : MonoBehaviour {
    protected void showMouse(bool show) {
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = show;
    }


    protected void sound(AudioClip audioClip, float volume = 2f) {
        if (audioClip != null) {
            AudioSource.PlayClipAtPoint(audioClip, transform.position, volume);
        }
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
