using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioActionKeyboardToggle))]
public class AudioActionKeyboardToggleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AudioActionKeyboardToggle script = (AudioActionKeyboardToggle)target;
        if (GUILayout.Button("Play"))
        {
            script.PlayEvent();
        }
        if (GUILayout.Button("Stop"))
        {
            script.StopEvent();
        }
    }
}
