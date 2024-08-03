using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

public class RTPCSynthExposeParameter : MonoBehaviour
{
    [System.Serializable]
    public class CSVData
    {
        public string Parameter;
        public float Value;
        public float MinRandomRange; 
        public float MaxRandomRange;
    }

    [System.Serializable]
    public class CSVFile
    {
        public string fileName;
        [HideInInspector] public bool loaded = false;
        public bool loadNow = false;
        public List<CSVData> data = new List<CSVData>();
    }

    [System.Serializable]
    public class AnimationEventInfo
    {
        public AnimationClip clip;
        public List<float> times = new List<float>();
        public List<int> csvFileIndices = new List<int>();
    }

    public List<CSVFile> csvFiles = new List<CSVFile>();
    public List<AnimationEventInfo> animationEvents = new List<AnimationEventInfo>();

    private void Start()
    {
        SetAnimationEvents(animationEvents);
    }

    private void Update()
    {
        foreach (CSVFile file in csvFiles)
        {
            if (file.loadNow && !file.loaded)
            {
                LoadCSV(file);
            }
        }
    }

    private void LoadCSV(CSVFile csvFile)
    {
        if (string.IsNullOrEmpty(csvFile.fileName))
        {
            Debug.LogError("CSV file name is not specified.");
            return;
        }
        string filePath = Path.Combine(Application.streamingAssetsPath, csvFile.fileName);
        if (!File.Exists(filePath))
        {
            Debug.LogError("CSV file not found at path: " + filePath);
            return;
        }
        foreach (CSVFile file in csvFiles)
        {
            if (file != csvFile)
            {
                file.loadNow = false;
                file.data.Clear();
                file.loaded = false;
            }
        }
        Debug.Log("CSV file loaded: " + csvFile.fileName);
        ReadCSV(filePath, csvFile);
        csvFile.loaded = true;
        SendValuesToWwise(csvFile);
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    private void ReadCSV(string filePath, CSVFile csvFile)
    {
        Debug.Log("Reading CSV file: " + filePath);
        string[] rows = File.ReadAllLines(filePath);
        Debug.Log("Number of lines in CSV file: " + rows.Length);
        foreach (string row in rows)
        {
            Debug.Log("CSV row: " + row);
            string[] columns = row.Split(',');
            if (columns.Length == 5) 
            {
                CSVData data = new CSVData
                {
                    
                    Parameter = columns[1].Trim(),
                    MinRandomRange = float.Parse(columns[3].Trim(), CultureInfo.InvariantCulture),
                    MaxRandomRange = float.Parse(columns[4].Trim(), CultureInfo.InvariantCulture)
                };
                float value;
                string valueStr = columns[2].Trim();
                if (valueStr == "0.000000")
                {
                    value = 0.0f;
                }
                else if (float.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                {
                    data.Value = value;
                    csvFile.data.Add(data);
                }
                else
                {
                    Debug.LogWarning("Failed to parse value: " + valueStr);
                }
            }
            else
            {
                Debug.LogWarning("Row format is incorrect: " + row);
            }
        }
        Debug.Log("CSV data loaded. Number of entries: " + csvFile.data.Count);
        Debug.Log("CSV data:");
        foreach (CSVData data in csvFile.data)
        {
            Debug.Log(" Parameter: " + data.Parameter + ", Value: " + data.Value);
        }
    }

    private void SendValuesToWwise(CSVFile csvFile)
    {
        foreach (CSVData data in csvFile.data)
        {
            float randomizedValue = UnityEngine.Random.Range(data.Value + data.MinRandomRange, data.Value + data.MaxRandomRange);
            string formattedValue = randomizedValue.ToString("0.000000", CultureInfo.InvariantCulture);
            AkSoundEngine.SetRTPCValue(data.Parameter, float.Parse(formattedValue, CultureInfo.InvariantCulture));
        }
    }

    void SetAnimationEvents(List<AnimationEventInfo> animationEvents)
    {
        foreach (var eventInfo in animationEvents)
        {
            if (eventInfo.clip != null && eventInfo.times.Count > 0 && eventInfo.times.Count == eventInfo.csvFileIndices.Count)
            {
                for (int i = 0; i < eventInfo.times.Count; i++)
                {
                    AnimationEvent evt = new AnimationEvent();
                    evt.time = eventInfo.times[i];
                    evt.functionName = "LoadCSVFromAnimation";
                    evt.intParameter = eventInfo.csvFileIndices[i];
                    eventInfo.clip.AddEvent(evt);
                }
            }
            else
            {
                Debug.LogWarning("AnimationEventInfo is not properly set.");
            }
        }
    }

    void LoadCSVFromAnimation(int csvFileIndex)
    {
        if (csvFileIndex >= 0 && csvFileIndex < csvFiles.Count)
        {
            csvFiles[csvFileIndex].loadNow = true;
        }
    }
}
