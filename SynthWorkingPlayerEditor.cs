using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SynthWorkingPlayer))]
public class SynthWorkingPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SynthWorkingPlayer script = (SynthWorkingPlayer)target;


        if (GUILayout.Button("Play"))
        {
            script.Play();
        }

        if (GUILayout.Button("Stop"))
        {
            script.Stop();
        }

    }
}
