using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RTPCCurveGenerator))]
public class RTPCSyncCurveEditor : Editor
{
    private RTPCCurveGenerator rtpcSyncAnimation;
    private float time;
    private bool playing;
    private float playbackSpeed = 1f;
    private string fileName = "curveValues";
    private void OnEnable()
    {rtpcSyncAnimation = (RTPCCurveGenerator)target;}
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (rtpcSyncAnimation.selectedAnimation == null)
        {EditorGUILayout.HelpBox("Please assign an AnimationClip.", MessageType.Warning);return;}
        time = EditorGUILayout.FloatField("Time", time);
        playbackSpeed = EditorGUILayout.FloatField("Playback Speed", playbackSpeed);
        if (GUILayout.Button(playing ? "Pause" : "Play"))
        {playing = !playing;EditorApplication.update -= OnEditorUpdate;
        if (playing){ EditorApplication.update += OnEditorUpdate;}}
        EditorGUILayout.Space();
        if (GUILayout.Button("Select Curve"))
        {ShowCurveSelectionMenu();}
        if (!string.IsNullOrEmpty(rtpcSyncAnimation.curveName))
        {EditorGUILayout.LabelField($"Selected Curve: {rtpcSyncAnimation.curveName}");}
        EditorGUILayout.Space();
        fileName = EditorGUILayout.TextField("File Name", fileName);
        if (GUILayout.Button("Generate Curve Values"))
        {GenerateCurveValues();}
    }
    private void ShowCurveSelectionMenu()
    {
        var menu = new GenericMenu();
        var bindings = AnimationUtility.GetCurveBindings(rtpcSyncAnimation.selectedAnimation);
        foreach (var binding in bindings)
        {if (AnimationUtility.GetEditorCurve(rtpcSyncAnimation.selectedAnimation, binding) != null){string curveName = binding.propertyName;menu.AddItem(new GUIContent(curveName), false, () => rtpcSyncAnimation.curveName = curveName);}}
        menu.ShowAsContext();
    }
    private void GenerateCurveValues()
    {   float currentTime = time;
        rtpcSyncAnimation.UpdateRTPCs(currentTime);
        string saveFilePath = EditorUtility.SaveFilePanel("Save Curve Values CSV", Application.streamingAssetsPath, fileName, "csv");
        if (!string.IsNullOrEmpty(saveFilePath)){ string fileName = System.IO.Path.GetFileName(saveFilePath);rtpcSyncAnimation.GenerateFinalCSV(fileName);AssetDatabase.Refresh();}}
    private void OnEditorUpdate()
    {   if (playing)
        {float deltaTime = Time.deltaTime * playbackSpeed;
         time += deltaTime;
         time = Mathf.Clamp(time, 0f, rtpcSyncAnimation.selectedAnimation.length);
         Repaint();
         if (time >= rtpcSyncAnimation.selectedAnimation.length){playing = false;EditorApplication.update -= OnEditorUpdate;}}
    }
}
