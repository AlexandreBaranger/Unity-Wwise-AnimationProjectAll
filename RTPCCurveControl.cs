using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class RTPCCurveControl : MonoBehaviour
{
    [System.Serializable]
    public class CSVFile
    {
        public string fileName;
        [HideInInspector] public bool loaded = false;
        public bool loadNow = false;
        public float triggerTime;
    }

    [SerializeField] public List<CSVFile> csvFiles = new List<CSVFile>();
    [SerializeField] public AK.Wwise.RTPC rtpc;
    [SerializeField] public List<KeyValuePair<float, float>> animationData = new List<KeyValuePair<float, float>>();
    [SerializeField] public List<float> interpolatedValues = new List<float>();
    [SerializeField] public float currentRTPCValue = 0f;
    [SerializeField] private bool enableDebugLogs = true;

    private float currentTime = 0f; // Ajout de la variable de temps courant

    void Start()
    {
        foreach (CSVFile file in csvFiles)
        {
            StartCoroutine(TriggerCSVLoad(file));
        }
    }

    void Update()
    {
        currentTime += Time.deltaTime; // Mise à jour du temps courant
        foreach (CSVFile file in csvFiles)
        {
            if (file.loadNow && !file.loaded)
            {
                StartCoroutine(LoadAndPlayCSV(file));
            }
        }
    }

    private IEnumerator TriggerCSVLoad(CSVFile file)
    {
        yield return new WaitForSeconds(file.triggerTime);
        file.loadNow = true;
    }

    public void PlayRTPCCurve(CSVFile file)
    {
        if (enableDebugLogs) Debug.Log("Playing RTPC Animation for file: " + file.fileName);
        StartCoroutine(LoadAndPlayCSV(file));
    }

    private IEnumerator LoadAndPlayCSV(CSVFile csvFile)
    {
        yield return LoadCSV(csvFile);
        CalculateInterpolatedValues();
        yield return StartCoroutine(CurveRTPC());
    }

    private IEnumerator LoadCSV(CSVFile csvFile)
    {
        animationData.Clear();
        string csvFilePath = Path.Combine(Application.streamingAssetsPath, "Audio", csvFile.fileName);

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
            if (!File.Exists(csvFilePath))
            {
                Debug.LogError("CSV file not found at path: " + csvFilePath);
                yield break;
            }

            string csvText = File.ReadAllText(csvFilePath);
            ProcessCSV(csvText);
        }

        csvFile.loaded = true; // Marque le fichier comme chargé
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

        if (interpolatedValues.Count > 0)
        {
            rtpc.SetValue(gameObject, interpolatedValues[interpolatedValues.Count - 1]);
            currentRTPCValue = interpolatedValues[interpolatedValues.Count - 1];
            if (enableDebugLogs) Debug.Log($"Final RTPC Value: {currentRTPCValue}");
        }
    }
}
