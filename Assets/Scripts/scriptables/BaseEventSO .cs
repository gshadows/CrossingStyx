using UnityEngine;
using UnityEngine.Events;

/**
 * Base event with arguments.
 */
public class BaseEventSO<T> : ScriptableObject
{
    [SerializeField] protected UnityEvent<T> onEventRaised;

    public void raise(T arg) {
        if (onEventRaised != null) onEventRaised.Invoke(arg);
    }

    public void addListener(UnityAction<T> listener) {
        if (listener != null) {
            onEventRaised.AddListener(listener);
        }
    }
}
