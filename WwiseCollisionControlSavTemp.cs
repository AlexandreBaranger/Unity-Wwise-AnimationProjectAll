using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using EventWwise = AK.Wwise.Event;
using System.Collections;
/*
[System.Serializable]
public class CollisionEventData
{
    public GameObject gameObject;
    public float distance;
    public float speed;
}

public class WwiseCollisionControlSavTemp : MonoBehaviour
{
    public bool debugCollisions = false;
    public ColliderCollisionHandler collisionHandler;
    public CollisionEvent[] collisionEvents;
    public float collisionCheckInterval = 0.1f;
    private float timeSinceLastCollisionCheck = 0f;

    void Start()
    {
        foreach (CollisionEvent collisionEvent in collisionEvents)
        {
            collisionEvent.CreateColliders();
            collisionEvent.CheckCollisions(collisionHandler);
        }
    }

    void Update()
    {
        timeSinceLastCollisionCheck += Time.deltaTime;
        if (timeSinceLastCollisionCheck >= collisionCheckInterval)
        {
            timeSinceLastCollisionCheck = 0f;
            foreach (CollisionEvent collisionEvent in collisionEvents)
            {
                collisionEvent.CheckCollisions(collisionHandler);
            }
        }
    }
}

[System.Serializable]
public class CollisionEvent
{
    public EventWwise wwiseEvent;
    public EventWwise secondWwiseEvent;  // Second Wwise event
    public float delayBeforeSecondEvent = 1.0f; // Delay before triggering the second event
    public float minDistance;
    public float maxDistance;
    public float minSpeed;
    public float maxSpeed;
    public string csvFileName;
    public List<GameObject> gameObjectsToSync = new List<GameObject>();
    public LayerMask collisionLayer;
    public float colliderRadius = 1.0f;
    public List<CollisionEventData> collisionDataList = new List<CollisionEventData>();
    public List<CSVData> csvDataList = new List<CSVData>();

    public void CreateColliders()
    {
        foreach (GameObject go in gameObjectsToSync)
        {
            SphereCollider collider = go.AddComponent<SphereCollider>();
            collider.radius = colliderRadius;
            collider.isTrigger = true;
        }
    }

    public void CheckCollisions(ColliderCollisionHandler collisionHandler)
    {
        foreach (GameObject go in gameObjectsToSync)
        {
            Collider[] colliders = Physics.OverlapSphere(go.transform.position, colliderRadius, collisionLayer);
            foreach (Collider other in colliders)
            {
                if (other.gameObject != go)
                {
                    float distance = Vector3.Distance(go.transform.position, other.transform.position);
                    Rigidbody otherRigidbody = other.attachedRigidbody;
                    float speed = (otherRigidbody != null) ? otherRigidbody.velocity.magnitude : 0f;
                    if (distance >= minDistance && distance <= maxDistance &&
                        speed >= minSpeed && speed <= maxSpeed)
                    {
                        Debug.Log($"Collision detected with {other.gameObject.name}: Distance = {distance}, Speed = {speed}");
                        CollisionEventData collisionData = new CollisionEventData();
                        collisionData.gameObject = other.gameObject;
                        collisionData.distance = distance;
                        collisionData.speed = speed;
                        collisionDataList.Add(collisionData);
                        collisionHandler?.HandleCollision(collisionData);
                        wwiseEvent.Post(go);
                        LoadCSVFromCollisionEvent(csvFileName);
                        if (secondWwiseEvent != null)
                        {
                            GameObject.FindObjectOfType<WwiseCollisionControl>().StartCoroutine(TriggerSecondEventAfterDelay(go, delayBeforeSecondEvent));
                        }
                    }
                }
            }
        }
    }

    private IEnumerator TriggerSecondEventAfterDelay(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        secondWwiseEvent.Post(go);
    }

    private void LoadCSVFromCollisionEvent(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        WwiseCollisionControl wwiseCollisionControl = GameObject.FindObjectOfType<WwiseCollisionControl>();
        if (File.Exists(filePath))
        {
            wwiseCollisionControl.StartCoroutine(LoadCSV(filePath));
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
*/