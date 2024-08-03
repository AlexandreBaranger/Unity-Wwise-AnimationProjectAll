using UnityEngine;
using AK.Wwise;
using System.IO;
using UnityEditor;
public class RTPCSynthPresetState: MonoBehaviour
{
    public enum PresetType
    {
        Preset1,
        Preset2,
        Preset3,
    }

    [System.Serializable]


    public class Preset
    {
        [Range(-96f, 0f)]
        public float sliderRtpcVolume;
        [Range(0f, 100f)]
        public float sliderRtpcHighPass;
        [Range(0f, 100f)]
        public float sliderRtpcLowPass;
        [Range(0f, 100f)]
        public float sliderRtpcPitch;
        [Range(0f, 100f)]
        public float sliderRtpcTranspose;
        [Range(0f, 100f)]
        public float sliderRtpcSpeedPlay;
        [Range(-1f, 1f)]
        public float sliderRtpcAttackCurve;
        [Range(-1f, 1f)]
        public float sliderRtpcAttackTime;
        [Range(-1f, 1f)]
        public float sliderRtpcDecayTime;
      
    }

    public AK.Wwise.RTPC rtpcVolume;
    public AK.Wwise.RTPC rtpcHighPass;
    public AK.Wwise.RTPC rtpcLowPass;
    public AK.Wwise.RTPC rtpcPitch;
    public AK.Wwise.RTPC rtpcTranspose;
    public AK.Wwise.RTPC rtpcSpeedPlay;
    public AK.Wwise.RTPC rtpcAttackCurve;
    public AK.Wwise.RTPC rtpcAttackTime;
    public AK.Wwise.RTPC rtpcDecayTime;


    public Preset preset1;
    public Preset preset2;
    public Preset preset3;


    [SerializeField]
    private bool preset1Selected;
    [SerializeField]
    private bool preset2Selected;
    [SerializeField]
    private bool preset3Selected;


    private bool previousPreset1Selected;
    private bool previousPreset2Selected;
    private bool previousPreset3Selected;

    [System.Serializable]
    public class AnimationEventInfo
    {
        public AnimationClip clip;
        public float[] times;
        public PresetType[] presets; // Use enum instead of string
    }

    public AnimationEventInfo preset1Animation;
    public AnimationEventInfo preset2Animation;
    public AnimationEventInfo preset3Animation;


    [Range(0f, 100f)]
    public float volumeRandomRange = 2f;
    [Range(0f, 100f)]
    public float highPassRandomRange = 2f;
    [Range(0f, 100f)]
    public float lowPassRandomRange = 2f;
    [Range(0f, 100f)]
    public float pitchRandomRange = 2f;
    [Range(0f, 100f)]
    public float transposeRandomRange = 2f;
    [Range(0f, 100f)]
    public float speedPlayRandomRange = 2f;
    [Range(0f, 1f)]
    public float attackCurveRandomRange = 0.1f;
    [Range(0f, 1f)]
    public float attackTimeRandomRange = 0.1f;
    [Range(0f, 1f)]
    public float decayTimeRandomRange = 0.1f;
    [Range(0f, 1f)]


    public bool Preset1
    {
        get => preset1Selected;
        set
        {
            preset1Selected = value;
            if (value)
            {
                ApplyPreset(preset1);
                Preset2 = false;
                Preset3 = false;
            }
        }
    }

    public bool Preset2
    {
        get => preset2Selected;
        set
        {
            preset2Selected = value;
            if (value)
            {
                ApplyPreset(preset2);
                Preset1 = false;
                Preset3 = false;
            }
        }
    }

    public bool Preset3
    {
        get => preset3Selected;
        set
        {
            preset3Selected = value;
            if (value)
            {
                ApplyPreset(preset3);
                Preset1 = false;
                Preset2 = false;
            }
        }
    }



    private void Update()
    {
        if (preset1Selected && !previousPreset1Selected)
        {
            ApplyPreset(preset1);
        }
        else if (preset2Selected && !previousPreset2Selected)
        {
            ApplyPreset(preset2);
        }
        else if (preset3Selected && !previousPreset3Selected)
        {
            ApplyPreset(preset3);
        }
        
        previousPreset1Selected = preset1Selected;
        previousPreset2Selected = preset2Selected;
        previousPreset3Selected = preset3Selected;
    }


    private void ApplyPreset(Preset preset)
    {
        float volume = preset.sliderRtpcVolume + Random.Range(-volumeRandomRange, volumeRandomRange);
        float highPass = preset.sliderRtpcHighPass + Random.Range(-highPassRandomRange, highPassRandomRange);
        float lowPass = preset.sliderRtpcLowPass + Random.Range(-lowPassRandomRange, lowPassRandomRange);
        float pitch = preset.sliderRtpcPitch + Random.Range(-pitchRandomRange, pitchRandomRange);
        float transpose = preset.sliderRtpcTranspose+ Random.Range(-transposeRandomRange, transposeRandomRange);
        float speedPlay = preset.sliderRtpcSpeedPlay + Random.Range(-speedPlayRandomRange, speedPlayRandomRange);
        float attackCurve = preset.sliderRtpcAttackCurve + Random.Range(-attackCurveRandomRange, attackCurveRandomRange);
        float attackTime = preset.sliderRtpcAttackTime + Random.Range(-attackTimeRandomRange, attackTimeRandomRange);
        float decayTime = preset.sliderRtpcDecayTime + Random.Range(-decayTimeRandomRange, decayTimeRandomRange);


        volume = Mathf.Clamp(volume, -96f, 0f);
        highPass = Mathf.Clamp(highPass, 0f, 100f);
        lowPass = Mathf.Clamp(lowPass, 0f, 100f);
        pitch = Mathf.Clamp(pitch, 0f, 100f);
        transpose = Mathf.Clamp(transpose, 0f, 100f);
        speedPlay = Mathf.Clamp(speedPlay, 0f, 100f);
        attackCurve = Mathf.Clamp(attackCurve, -1f, 1f);
        attackTime = Mathf.Clamp(attackTime, -1f, 1f);
        decayTime = Mathf.Clamp(decayTime, -1f, 1f);

        Debug.Log($"Applying preset: Volume = {volume}, Synth1 = {highPass}, Synth2 = {lowPass},Synth3P = {pitch}, Synth3T = {transpose}, Synth3S = {speedPlay}, Synth4 = {attackCurve}, Synth5 = {attackTime}, Synth6 = {decayTime}");

        rtpcVolume.SetGlobalValue(volume);
        rtpcHighPass.SetGlobalValue(highPass);
        rtpcLowPass.SetGlobalValue(lowPass);
        rtpcPitch.SetGlobalValue(pitch);
        rtpcTranspose.SetGlobalValue(transpose);
        rtpcSpeedPlay.SetGlobalValue(speedPlay);
        rtpcAttackCurve.SetGlobalValue(attackCurve);
        rtpcAttackTime.SetGlobalValue(attackTime);
        rtpcDecayTime.SetGlobalValue(decayTime);
    }

    public void ApplyPresetByType(PresetType presetType)
    {
        switch (presetType)
        {
            case PresetType.Preset1:
                Preset1 = true;
                break;
            case PresetType.Preset2:
                Preset2 = true;
                break;
            case PresetType.Preset3:
                Preset3 = true;
                break;

            default:
                Debug.LogWarning($"Preset '{presetType}' not found.");
                break;
        }
    }

    void Start()
    {
        SetAnimationEvents(preset1Animation);
        SetAnimationEvents(preset2Animation);
        SetAnimationEvents(preset3Animation);
    }

    void SetAnimationEvents(AnimationEventInfo animationEventInfo)
    {
        if (animationEventInfo.clip != null && animationEventInfo.presets.Length == animationEventInfo.times.Length)
        {
            for (int i = 0; i < animationEventInfo.times.Length; i++)
            {
                AnimationEvent evt = new AnimationEvent();
                evt.time = animationEventInfo.times[i];
                evt.functionName = "InvokePreset";
                evt.intParameter = (int)animationEventInfo.presets[i];
                animationEventInfo.clip.AddEvent(evt);
            }
        }
        else
        {
            Debug.LogWarning("AnimationEventInfo times and presets length mismatch or clip is null");
        }
    }

    void InvokePreset(int presetIndex)
    {
        ApplyPresetByType((PresetType)presetIndex);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RTPCSynthPresetState))]
    public class RTPCSynthPresetStateEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            RTPCSynthPresetState script = (RTPCSynthPresetState)target;

            if (GUILayout.Button("Save Preset to CSV"))
            {
                SavePresetToCSV(script);
            }
        }

        private string FormatFloat(float value)
        {
            return value.ToString("F6").Replace(',', '.');
        }

        private void SavePresetToCSV(RTPCSynthPresetState script)
        {
            string path = EditorUtility.SaveFilePanel("Save Preset", "", "preset.csv", "csv");
            if (string.IsNullOrEmpty(path)) return;

            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("Volume" + "," + script.rtpcVolume + "," + FormatFloat(script.rtpcVolume.GetValue(script.gameObject))+ "," + "0.0" + "," + "0.0");
                writer.WriteLine("HighPass" + "," + script.rtpcHighPass + "," + FormatFloat(script.rtpcHighPass.GetValue(script.gameObject))+ "," + "0.0" + "," + "0.0");
                writer.WriteLine("LowPass" + "," + script.rtpcLowPass + "," + FormatFloat(script.rtpcLowPass.GetValue(script.gameObject))+ "," + "0.0" + "," + "0.0");
                writer.WriteLine("Pitch " + "," + script.rtpcPitch + "," + FormatFloat(script.rtpcPitch.GetValue(script.gameObject)) + "," + "0.0" + "," + "0.0");
                writer.WriteLine("Transpose " + "," + script.rtpcTranspose + "," + FormatFloat(script.rtpcTranspose.GetValue(script.gameObject)) + "," + "0.0" + "," + "0.0");
                writer.WriteLine("SpeedPlay " + "," + script.rtpcSpeedPlay + "," + FormatFloat(script.rtpcSpeedPlay.GetValue(script.gameObject))+  "," + "0.0" + "," + "0.0");
                writer.WriteLine("AttackCurve " + "," + script.rtpcAttackCurve + "," + FormatFloat(script.rtpcAttackCurve.GetValue(script.gameObject))+ "," + "0.0" + "," + "0.0");
                writer.WriteLine("AttackTime" + "," + script.rtpcAttackTime + "," + FormatFloat(script.rtpcAttackTime.GetValue(script.gameObject))+ "," + "0.0" + "," + "0.0");
                writer.WriteLine("DecayTime" + "," + script.rtpcDecayTime + "," + FormatFloat(script.rtpcDecayTime.GetValue(script.gameObject))+ "," + "0.0" + "," + "0.0");
            }

            Debug.Log("Preset saved to " + path);
            AssetDatabase.Refresh();
        }
    }
#endif
}

