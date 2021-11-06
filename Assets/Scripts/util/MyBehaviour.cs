using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Localization;

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
            if (obj.GetComponent<MonoBehaviour>() != null) {
                if (obj.activeSelf) {
                    obj.SendMessage(method, param, SendMessageOptions.DontRequireReceiver);
                } else {
                    //Debug.LogFormat("Trying to invoke {0} on disabled object {1}...", method, obj.name);
                    invokeOnAllBehaviours(obj, method, param);
                }
            }
        }
    }


    protected void invokeOnAllBehaviours(GameObject obj, string method, object param = null) {
        try {
            foreach (var component in obj.GetComponents<MonoBehaviour>()) {
                //Debug.LogFormat("Trying to invoke {0}.{1}...", component.GetType().Name, method);
                var methodInfo = component.GetType().GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (methodInfo != null) {
                    int numParams = methodInfo.GetParameters().Length;
                    Debug.LogFormat("Invoke reflex: pub:{0}, priv:{1}, params {2}", methodInfo.IsPublic, methodInfo.IsPrivate, numParams);
                    if (numParams == 0) {
                        methodInfo.Invoke(component, null);
                    } else if (numParams == 1) {
                        methodInfo.Invoke(component, new object[] { param });
                    } else {
                        Debug.LogWarningFormat("Can not invoke {0}.{1} because it expects {2} params!", component.GetType().Name, method, numParams);
                    }
                } else {
                    Debug.LogWarningFormat("Method {0}.{1} not found", component.GetType(), method);
                }
            }
        }
        catch (Exception ex) {
            Debug.LogErrorFormat("Failed bcast reflexion invoke with {0}", ex);
        }
    }


    protected static string SafeLocalizedStr (LocalizedString lstr, string fallback = "", params object[] args) {
        try {
            string str = lstr.GetLocalizedString(args);
            if (str == null) str = fallback;
            return str;
        }
        catch (Exception ex) {
            Debug.LogWarning("Failed to resolve string: " + ex.Message);
            return fallback;
        }
    }
}
