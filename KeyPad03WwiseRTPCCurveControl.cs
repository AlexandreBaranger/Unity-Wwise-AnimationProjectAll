using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using AK.Wwise;
using UnityEngine.Networking;
using System.IO;

public class KeyPad03WwiseRTPCCurveControl : MonoBehaviour
{
    public float delayInSeconds = 5f;
    [SerializeField] public AK.Wwise.RTPC rtpc;
    [SerializeField] public string csvFileName = "";
    [SerializeField] public List<KeyValuePair<float, float>> animationData = new List<KeyValuePair<float, float>>();
    [SerializeField] public List<float> interpolatedValues = new List<float>();
    [SerializeField] public float currentRTPCValue = 0f;
    [SerializeField] private bool enableDebugLogs = true;

    void Start()
    {
        if (delayInSeconds > 0)
        {
            StartCoroutine(LoadAndPlayCSV());
        }
    }

    void Update()
    {

        if (Input.GetKey(KeyCode.RightShift))
        {
            PlayRTPCCurve();
        }

        if (Input.GetKey(KeyCode.Keypad3))
        {
            PlayRTPCCurve(); 
        }
    }

    public void PlayRTPCCurve()
    {
        if (enableDebugLogs) Debug.Log("Playing RTPC Animation...");
        StartCoroutine(LoadAndPlayCSV());
    }

    private IEnumerator LoadAndPlayCSV()
    {
        yield return new WaitForSeconds(delayInSeconds);
        yield return LoadCSV();
        CalculateInterpolatedValues();
        yield return StartCoroutine(CurveRTPC());
    }

    private IEnumerator LoadCSV()
    {
        animationData.Clear();
        string csvFilePath = Path.Combine(Application.streamingAssetsPath, "Audio", csvFileName);

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
                ProcessCSV(csvText);
            }
        }
        else
        {
            string csvText = File.ReadAllText(csvFilePath);
            ProcessCSV(csvText);
        }
    }

    private void ProcessCSV(string csvText)
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
                    animationData.Add(new KeyValuePair<float, float>(time, rtpcValue));
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

        if (enableDebugLogs) Debug.Log($"Total lines loaded: {animationData.Count}");
    }

    private void CalculateInterpolatedValues()
    {
        interpolatedValues.Clear();

        if (enableDebugLogs) Debug.Log($"animationData.Count: {animationData.Count}");

        for (int i = 0; i < animationData.Count - 1; i++)
        {
            float startTime = animationData[i].Key;
            float endTime = animationData[i + 1].Key;
            float startValue = animationData[i].Value;
            float endValue = animationData[i + 1].Value;

            if (enableDebugLogs) Debug.Log($"startTime: {startTime}, endTime: {endTime}, startValue: {startValue}, endValue: {endValue}");

            if (Mathf.Sign(startValue) != Mathf.Sign(endValue))
            {
                float midTime = (startTime + endTime) / 2f;
                float midValue = Mathf.Abs(startValue) < Mathf.Abs(endValue) ? startValue : endValue;

                InterpolateSegment(startTime, midTime, startValue, midValue);
                InterpolateSegment(midTime, endTime, midValue, endValue);
            }
            else
            {
                InterpolateSegment(startTime, endTime, startValue, endValue);
            }
        }
    }

    private void InterpolateSegment(float startTime, float endTime, float startValue, float endValue)
    {
        int steps = Mathf.CeilToInt((endTime - startTime) * 100);

        for (int j = 0; j <= steps; j++)
        {
            float t = (float)j / steps;
            float interpolatedValue = Mathf.Lerp(startValue, endValue, t);
            interpolatedValues.Add(interpolatedValue);
            if (enableDebugLogs) Debug.Log($"Interpolated Value at {startTime + (t * (endTime - startTime))} ms: {interpolatedValue}");
        }
    }

    private IEnumerator CurveRTPC()
    {
        float startTime = Time.time;
        int index = 0;

        while (index < interpolatedValues.Count)
        {
            float currentTime = Time.time - startTime;
            rtpc.SetValue(gameObject, interpolatedValues[index]);
            currentRTPCValue = interpolatedValues[index];
            if (enableDebugLogs) Debug.Log($"Time: {currentTime}, RTPC Value: {currentRTPCValue}");

            index++;
            yield return null;
        }
 
    {
        if (interpolatedValues.Count > 0)
        {
            rtpc.SetValue(gameObject, interpolatedValues[interpolatedValues.Count - 1]);
            currentRTPCValue = interpolatedValues[interpolatedValues.Count - 1];
            if (enableDebugLogs) Debug.Log($"Final RTPC Value: {currentRTPCValue}");
        }
    }
        if (interpolatedValues.Count > 0)
        {
            rtpc.SetValue(gameObject, interpolatedValues[interpolatedValues.Count - 1]);
            currentRTPCValue = interpolatedValues[interpolatedValues.Count - 1];
            if (enableDebugLogs) Debug.Log($"Final RTPC Value: {currentRTPCValue}");
        }
    }
}
