using UnityEngine;
using UnityEngine.Events;

/**
 * Base event without arguments.
 */
[CreateAssetMenu(menuName = "EventSO/Void Event")]
public class VoidEventSO : ScriptableObject
{
    [SerializeField] protected UnityEvent onEventRaised;

    public void raise() {
        if (onEventRaised != null) onEventRaised.Invoke();
    }

    public void addListener(UnityAction listener) {
        if (listener != null) {
            onEventRaised.AddListener(listener);
        }
    }
}
