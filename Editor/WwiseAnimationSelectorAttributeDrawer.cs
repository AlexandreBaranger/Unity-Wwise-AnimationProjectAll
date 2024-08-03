using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(WwiseAnimationSelectorAttribute))]
public class WwiseAnimationSelectorAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label);
    }
}
