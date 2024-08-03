using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using AK.Wwise;
using UnityEngine.Networking;
using System.IO;

public class WwiseCollisionRTPCCurveControl : MonoBehaviour
{
    public float delayInSeconds = 5f;
    [SerializeField] public List<RTPCConfiguration> rtpcConfigurations = new List<RTPCConfiguration>();
    [SerializeField] private bool enableDebugLogs = true;

    [System.Serializable]
    public class RTPCConfiguration
    {
        public AK.Wwise.RTPC rtpc;
        public string csvFileName;
        public List<KeyValuePair<float, float>> animationData = new List<KeyValuePair<float, float>>();
        public List<float> interpolatedValues = new List<float>();
        public float currentRTPCValue = 0f;
    }

    void Start()
    {
        if (delayInSeconds > 0)
        {
            StartCoroutine(LoadAndPlayAllCSVs());
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.RightShift))
        {
            PlayAllRTPCCurves();
        }

        if (Input.GetKey(KeyCode.Keypad3))
        {
            PlayAllRTPCCurves();
        }
    }

    public void PlayAllRTPCCurves()
    {
        if (enableDebugLogs) Debug.Log("Playing all RTPC Animations...");
        StartCoroutine(LoadAndPlayAllCSVs());
    }

    private IEnumerator LoadAndPlayAllCSVs()
    {
        yield return new WaitForSeconds(delayInSeconds);

        foreach (var rtpcConfig in rtpcConfigurations)
        {
            yield return StartCoroutine(LoadCSV(rtpcConfig));
            CalculateInterpolatedValues(rtpcConfig);
            StartCoroutine(CurveRTPC(rtpcConfig));
        }
    }

    private IEnumerator LoadCSV(RTPCConfiguration rtpcConfig)
    {
        rtpcConfig.animationData.Clear();
        string csvFilePath = Path.Combine(Application.streamingAssetsPath, "Audio", rtpcConfig.csvFileName);

        if (csvFilePath.StartsWith("http://") || csvFilePath.StartsWith("https://"))
        {
            using (UnityWebRequest www = UnityWebRequest.Get(csvFilePath))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error loading CSV file: " + www.error);
                    yield break;
                }

                string csvText = www.downloadHandler.text;
                ProcessCSV(csvText, rtpcConfig);
            }
        }
        else
        {
            string csvText = File.ReadAllText(csvFilePath);
            ProcessCSV(csvText, rtpcConfig);
        }
    }

    private void ProcessCSV(string csvText, RTPCConfiguration rtpcConfig)
    {
        string[] lines = csvText.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            if (enableDebugLogs) Debug.Log($"Reading line: {line}");

            string[] values = line.Trim().Split('_');

            if (values.Length >= 2)
            {
                float time, rtpcValue;

                if (float.TryParse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out time) &&
                    float.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out rtpcValue))
                {
                    rtpcConfig.animationData.Add(new KeyValuePair<float, float>(time, rtpcValue));
                    if (enableDebugLogs) Debug.Log($"Loaded CSV line - Time: {time}, RTPC Value: {rtpcValue}");
                }
                else
                {
                    if (enableDebugLogs) Debug.LogWarning($"Failed to parse values - Time: {values[0]}, RTPC Value: {values[1]}");
                }
            }
            else
            {
                if (enableDebugLogs) Debug.LogWarning($"Unexpected line format: {line}");
            }
        }

        if (enableDebugLogs) Debug.Log($"Total lines loaded: {rtpcConfig.animationData.Count}");
    }

    private void CalculateInterpolatedValues(RTPCConfiguration rtpcConfig)
    {
        rtpcConfig.interpolatedValues.Clear();

        if (enableDebugLogs) Debug.Log($"animationData.Count: {rtpcConfig.animationData.Count}");

        for (int i = 0; i < rtpcConfig.animationData.Count - 1; i++)
        {
            float startTime = rtpcConfig.animationData[i].Key;
            float endTime = rtpcConfig.animationData[i + 1].Key;
            float startValue = rtpcConfig.animationData[i].Value;
            float endValue = rtpcConfig.animationData[i + 1].Value;

            if (enableDebugLogs) Debug.Log($"startTime: {startTime}, endTime: {endTime}, startValue: {startValue}, endValue: {endValue}");

            if (Mathf.Sign(startValue) != Mathf.Sign(endValue))
            {
                float midTime = (startTime + endTime) / 2f;
                float midValue = Mathf.Abs(startValue) < Mathf.Abs(endValue) ? startValue : endValue;

                InterpolateSegment(startTime, midTime, startValue, midValue, rtpcConfig);
                InterpolateSegment(midTime, endTime, midValue, endValue, rtpcConfig);
            }
            else
            {
                InterpolateSegment(startTime, endTime, startValue, endValue, rtpcConfig);
            }
        }
    }

    private void InterpolateSegment(float startTime, float endTime, float startValue, float endValue, RTPCConfiguration rtpcConfig)
    {
        int steps = Mathf.CeilToInt((endTime - startTime) * 100);

        for (int j = 0; j <= steps; j++)
        {
            float t = (float)j / steps;
            float interpolatedValue = Mathf.Lerp(startValue, endValue, t);
            rtpcConfig.interpolatedValues.Add(interpolatedValue);
            if (enableDebugLogs) Debug.Log($"Interpolated Value at {startTime + (t * (endTime - startTime))} ms: {interpolatedValue}");
        }
    }

    private IEnumerator CurveRTPC(RTPCConfiguration rtpcConfig)
    {
        float startTime = Time.time;
        int index = 0;

        while (index < rtpcConfig.interpolatedValues.Count)
        {
            float currentTime = Time.time - startTime;
            rtpcConfig.rtpc.SetValue(gameObject, rtpcConfig.interpolatedValues[index]);
            rtpcConfig.currentRTPCValue = rtpcConfig.interpolatedValues[index];
            if (enableDebugLogs) Debug.Log($"Time: {currentTime}, RTPC Value: {rtpcConfig.currentRTPCValue}");

            index++;
            yield return null;
        }

        if (rtpcConfig.interpolatedValues.Count > 0)
        {
            rtpcConfig.rtpc.SetValue(gameObject, rtpcConfig.interpolatedValues[rtpcConfig.interpolatedValues.Count - 1]);
            rtpcConfig.currentRTPCValue = rtpcConfig.interpolatedValues[rtpcConfig.interpolatedValues.Count - 1];
            if (enableDebugLogs) Debug.Log($"Final RTPC Value: {rtpcConfig.currentRTPCValue}");
        }
    }

    [ContextMenu("Play")]
    public void PlayEvent()
    {
        PlayAllRTPCCurves();
    }

    [ContextMenu("Stop")]
    public void StopEvent()
    {
        // Stop all RTPC curves
        StopAllCoroutines();
    }
}
