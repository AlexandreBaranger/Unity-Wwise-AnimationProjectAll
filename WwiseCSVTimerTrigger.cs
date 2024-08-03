using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

public class WwiseCSVTimerTrigger : MonoBehaviour
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
        public AK.Wwise.Event wwiseEvent;
        public string fileName;
        public GameObject targetObject; // Add this line
        [HideInInspector] public bool loaded = false;
        public bool loadNow = false;
        public float triggerTime;
        public List<CSVData> data = new List<CSVData>();
    }


    public List<CSVFile> csvFiles = new List<CSVFile>();
    public float currentTime = 0f;
    public float resetTime = 60f;

    private void Awake()
    {
        foreach (CSVFile file in csvFiles)
        {
            StartCoroutine(TriggerCSVLoad(file));
        }
    }

    private void Start()
    {
        StartCoroutine(ResetTimerAndReloadFiles());
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        foreach (CSVFile file in csvFiles)
        {
            if (file.loadNow && !file.loaded)
            {
                LoadCSV(file);
            }
            else if (!file.loadNow && file.loaded)
            {
                Debug.Log("CSV file already loaded and processed: " + file.fileName);
            }
        }
    }

    private IEnumerator TriggerCSVLoad(CSVFile file)
    {
        Debug.Log("Waiting for trigger time: " + file.triggerTime + " for file: " + file.fileName);
        yield return new WaitForSeconds(file.triggerTime);

        if (!string.IsNullOrEmpty(file.fileName) || file.wwiseEvent != null)
        {
            file.loadNow = true;
            Debug.Log("Setting loadNow to true for file: " + file.fileName);

            // If there is no CSV file, trigger the Wwise event directly
            if (string.IsNullOrEmpty(file.fileName))
            {
                TriggerWwiseEvent(file);
            }
        }
    }

    private void TriggerWwiseEvent(CSVFile file)
    {
        if (file.wwiseEvent == null)
        {
            Debug.LogError("Wwise Event is not assigned for file: " + file.fileName);
            return;
        }

        GameObject target = file.targetObject != null ? file.targetObject : gameObject;
        Debug.Log("Triggering Wwise event for file: " + file.fileName + " on GameObject: " + target.name);
        AkSoundEngine.PostEvent(file.wwiseEvent.Id, target);
    }


    private void SendValuesToWwise(CSVFile csvFile)
    {
        if (csvFile.wwiseEvent == null)
        {
            Debug.LogError("Wwise Event is not assigned for file: " + csvFile.fileName);
            return;
        }

        foreach (CSVData data in csvFile.data)
        {
            float randomizedValue = UnityEngine.Random.Range(data.Value + data.MinRandomRange, data.Value + data.MaxRandomRange);
            string formattedValue = randomizedValue.ToString("0.000000", CultureInfo.InvariantCulture);
            AkSoundEngine.SetRTPCValue(data.Parameter, float.Parse(formattedValue, CultureInfo.InvariantCulture));
            Debug.Log("Sending RTPC value: " + formattedValue + " for parameter: " + data.Parameter);
        }

        GameObject target = csvFile.targetObject != null ? csvFile.targetObject : gameObject;
        Debug.Log("Triggering Wwise event for file: " + csvFile.fileName + " on GameObject: " + target.name);
        AkSoundEngine.PostEvent(csvFile.wwiseEvent.Id, target);
    }


    private IEnumerator ResetTimerAndReloadFiles()
    {
        while (true)
        {
            yield return new WaitForSeconds(resetTime);
            currentTime = 0f;
            foreach (CSVFile file in csvFiles)
            {
                file.loaded = false;
                StartCoroutine(TriggerCSVLoad(file));
            }
        }
    }

    private void LoadCSV(CSVFile csvFile)
    {
        if (string.IsNullOrEmpty(csvFile.fileName) && csvFile.wwiseEvent == null)
        {
            Debug.LogError("CSV file name and Wwise Event are not specified for file: " + csvFile.fileName);
            csvFile.loadNow = false;
            return;
        }

        if (string.IsNullOrEmpty(csvFile.fileName))
        {
            csvFile.loaded = true;
            SendValuesToWwise(csvFile);
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
        foreach (CSVData data in csvFile.data)
        {
            Debug.Log("Parameter: " + data.Parameter + ", Value: " + data.Value);
        }
    }
}
