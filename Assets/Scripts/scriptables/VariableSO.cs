using UnityEngine;

public abstract class VariableSO<T> : ScriptableObject {
    [SerializeField] protected T defaultValue;
    [SerializeField] protected T runtimeValue;

    private void Awake() {
        reset();
    }

    public void reset() {
        runtimeValue = defaultValue;
    }

    public T get() {
        return runtimeValue;
    }

    public void set(T value) {
        runtimeValue = value;
    }

    public T getDefault() {
        return defaultValue;
    }
}
