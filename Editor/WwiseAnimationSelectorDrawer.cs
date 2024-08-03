using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

[CustomPropertyDrawer(typeof(WwiseAnimationSelectorAttribute))]
public class WwiseAnimationSelectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        WwiseAnimationSelectorAttribute methodSelector = (WwiseAnimationSelectorAttribute)attribute;
        var targetTypes = methodSelector.TargetTypes;

        if (targetTypes == null || targetTypes.Length == 0)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        var methods = targetTypes
            .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .Where(m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
            .Select(m => $"{m.DeclaringType.Name}.{m.Name}")
            .ToArray();

        int index = Mathf.Max(0, System.Array.IndexOf(methods, property.stringValue));
        index = EditorGUI.Popup(position, label.text, index, methods);
        property.stringValue = methods.Length > 0 ? methods[index] : string.Empty;
    }
}
