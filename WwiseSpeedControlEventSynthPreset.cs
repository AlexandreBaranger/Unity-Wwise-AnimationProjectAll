using UnityEngine;
using System.IO;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;

public class WwiseSpeedControlEventSynthPreset : MonoBehaviour
{
    public GameObject targetObject;
    public float updateInterval = 0.1f;
    public float maxSpeed = 50.0f;

    [System.Serializable]
    public class SpeedEventConfig
    {
        public AK.Wwise.Event wwiseEvent;
        public float minSpeed;
        public float maxSpeed;
        public float minHorizontalMovement;
        public float maxHorizontalMovement;
        public float minVerticalMovement;
        public float maxVerticalMovement;
        public string csvFileName;
    }

    [System.Serializable]
    public class StopEventConfig
    {
        public AK.Wwise.Event stopEvent;
        public float minStopSpeed;
    }

    [System.Serializable]
    public class RTPCConfig
    {
        public AK.Wwise.RTPC rtpc;
        [Range(-96.0f, 3200.0f)]
        public float rtpcDebugValue;
        public float minRTPCValue;
        public float maxRTPCValue;
    }

    public List<SpeedEventConfig> speedEvents;
    public StopEventConfig stopEventConfig;
    public List<RTPCConfig> rtpcConfigs;
    private float timeSinceLastUpdate = 0.0f;
    private Vector3 lastPosition;
    [SerializeField]
    private float currentSpeed = 0.0f;
    private bool isMoving = false;
    public List<CSVData> csvDataList = new List<CSVData>();

    private Dictionary<AK.Wwise.Event, bool> activeEvents = new Dictionary<AK.Wwise.Event, bool>();

    private void Start()
    {
        if (targetObject == null)
        {
            enabled = false;
            return;
        }
        lastPosition = targetObject.transform.position;

        foreach (var config in speedEvents)
        {
            activeEvents[config.wwiseEvent] = false;
        }
    }

    private void Update()
    {
        if (targetObject == null)
        {
            enabled = false;
            return;
        }

        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            Vector3 currentPosition = targetObject.transform.position;
            float speed = Vector3.Distance(currentPosition, lastPosition) / updateInterval;
            currentSpeed = Mathf.Clamp(speed, 0.0f, maxSpeed);
            float horizontalMovement = Mathf.Abs(currentPosition.x - lastPosition.x);
            float verticalMovement = Mathf.Abs(currentPosition.y - lastPosition.y);

            foreach (var rtpcConfig in rtpcConfigs)
            {
                float rtpcValue = MapSpeedToRTPC(currentSpeed, 0.0f, maxSpeed, rtpcConfig.minRTPCValue, rtpcConfig.maxRTPCValue);

                if (rtpcConfig.rtpc != null)
                {
                    rtpcConfig.rtpc.SetValue(gameObject, rtpcValue);
                    rtpcConfig.rtpcDebugValue = rtpcValue;
                }
            }

            bool eventTriggered = false;

            foreach (var config in speedEvents)
            {
                if (currentSpeed >= config.minSpeed && currentSpeed <= config.maxSpeed &&
                    horizontalMovement >= config.minHorizontalMovement && horizontalMovement <= config.maxHorizontalMovement &&
                    verticalMovement >= config.minVerticalMovement && verticalMovement <= config.maxVerticalMovement)
                {
                    if (!activeEvents[config.wwiseEvent])
                    {
                        config.wwiseEvent.Post(gameObject);
                        activeEvents[config.wwiseEvent] = true;
                        LoadCSVFromSpeedEvent(config.csvFileName);
                    }
                    eventTriggered = true;
                }
            }

            if (!eventTriggered && isMoving)
            {
                stopEventConfig.stopEvent.Post(gameObject);
                foreach (var config in speedEvents)
                {
                    activeEvents[config.wwiseEvent] = false;
                }
                isMoving = false;
            }
            else if (eventTriggered && !isMoving)
            {
                isMoving = true;
            }

            lastPosition = currentPosition;
            timeSinceLastUpdate = 0;
        }
    }

    private void LoadCSVFromSpeedEvent(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(filePath))
        {
            StartCoroutine(LoadCSV(filePath));
        }
    }

    private IEnumerator LoadCSV(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("CSV file not found at path: " + filePath);
            yield break;
        }
        string[] rows = File.ReadAllLines(filePath);
        yield return null;

        csvDataList.Clear();

        foreach (string row in rows)
        {
            string[] columns = row.Split(',');
            if (columns.Length == 5)
            {
                CSVData data = new CSVData
                {
                    Volume = columns[0].Trim(),
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
                    SendValueToWwise(data);
                }
                else
                {
                    Debug.LogWarning("Failed to parse value: " + valueStr);
                }

                csvDataList.Add(data);
            }
            else
            {
                Debug.LogWarning("Row format is incorrect: " + row);
            }
        }
    }

    private void SendValueToWwise(CSVData data)
    {
        float randomizedValue = UnityEngine.Random.Range(data.Value + data.MinRandomRange, data.Value + data.MaxRandomRange);
        string formattedValue = randomizedValue.ToString("0.000000", CultureInfo.InvariantCulture);
        AkSoundEngine.SetRTPCValue(data.Parameter, float.Parse(formattedValue, CultureInfo.InvariantCulture));
    }

    private float MapSpeedToRTPC(float speed, float minSpeed, float maxSpeed, float minRTPC, float maxRTPC)
    {
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        float rtpcValue = Mathf.Lerp(minRTPC, maxRTPC, (speed - minSpeed) / (maxSpeed - minSpeed));
        return rtpcValue;
    }

    [System.Serializable]
    public class CSVData
    {
        public string Volume;
        public string Parameter;
        public float Value;
        public float MinRandomRange;
        public float MaxRandomRange;
    }
}
