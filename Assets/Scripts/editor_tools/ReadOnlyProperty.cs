using UnityEditor;
using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute {}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyProperty : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        bool savedGuiEnable = GUI.enabled;
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = savedGuiEnable;
    }
}
#endif
