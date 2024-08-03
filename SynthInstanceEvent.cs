using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Globalization;

public class SynthInstanceEvent : MonoBehaviour
{
    public GameObject gameObjectToSync;
    public bool debugMode = false;
    private Vector3 previousPosition;
    public WwiseInstanceDataObject[] instanceDataArray;
    public float sphereRadius = 1.0f;
    public Color debugSphereColor = Color.gray; // Ajoutez ce champ pour choisir la couleur des sphères de débogage
    private Dictionary<string, bool> instanceFlags = new Dictionary<string, bool>();
    private Dictionary<string, GameObject> instances = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> debugSpheres = new Dictionary<string, GameObject>();
    private Dictionary<string, bool> enterFlags = new Dictionary<string, bool>();
    public string csvFileName;

    [SerializeField]
    public List<CSVDataInstance> csvDataList = new List<CSVDataInstance>();

    private void Start()
    {
        if (gameObjectToSync != null)
        {
            previousPosition = gameObjectToSync.transform.position;
        }
    }

    private void Update()
    {
        if (gameObjectToSync != null)
        {
            Vector3 velocity = (gameObjectToSync.transform.position - previousPosition) / Time.deltaTime;
            transform.position = gameObjectToSync.transform.position;

            foreach (WwiseInstanceDataObject data in instanceDataArray)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, sphereRadius);
                HashSet<string> foundObjects = new HashSet<string>();

                foreach (Collider collider in colliders)
                {
                    GameObject obj = collider.gameObject;

                    foreach (GameObject targetObject in data.targetObjects)
                    {
                        if (obj == targetObject)
                        {
                            foundObjects.Add(targetObject.name);

                            if (!instanceFlags.ContainsKey(targetObject.name) || !instanceFlags[targetObject.name])
                            {
                                GameObject instance = Instantiate(targetObject, obj.transform.position, Quaternion.identity);
                                instances[targetObject.name] = instance;
                                instanceFlags[targetObject.name] = true;

                                if (data.instantiateEvent != null)
                                {
                                    data.instantiateEvent.Post(instance);
                                    LoadCSVInstanceEvent(csvFileName);
                                }

                                if (debugMode && !debugSpheres.ContainsKey(targetObject.name))
                                {
                                    GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                    debugSphere.transform.localScale = Vector3.one * sphereRadius;
                                    // Créez un nouveau matériau pour chaque sphère de débogage
                                    Renderer renderer = debugSphere.GetComponent<Renderer>();
                                    renderer.material = new Material(renderer.material);
                                    renderer.material.color = debugSphereColor;
                                    debugSphere.transform.parent = instance.transform;  // Attachez la sphère de débogage à l'instance
                                    debugSpheres[targetObject.name] = debugSphere;
                                }
                            }
                        }
                    }
                }

                foreach (GameObject targetObject in data.targetObjects)
                {
                    if (!foundObjects.Contains(targetObject.name))
                    {
                        if (instanceFlags.ContainsKey(targetObject.name) && instanceFlags[targetObject.name])
                        {
                            instanceFlags[targetObject.name] = false;
                            if (instances.ContainsKey(targetObject.name))
                            {
                                Destroy(instances[targetObject.name]);
                                instances.Remove(targetObject.name);
                            }

                            if (debugSpheres.ContainsKey(targetObject.name))
                            {
                                Destroy(debugSpheres[targetObject.name]);
                                debugSpheres.Remove(targetObject.name);
                            }

                            if (data.destroyEvent != null)
                            {
                                data.destroyEvent.Post(gameObject);
                            }
                        }
                    }
                }
            }

            previousPosition = gameObjectToSync.transform.position;
        }

        CheckInactiveObjects(); // Ajoutez cette ligne pour vérifier continuellement les objets inactifs
    }

    private void LoadCSVInstanceEvent(string fileName)
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
                CSVDataInstance data = new CSVDataInstance
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

    private void SendValueToWwise(CSVDataInstance data)
    {
        float randomizedValue = UnityEngine.Random.Range(data.Value + data.MinRandomRange, data.Value + data.MaxRandomRange);
        string formattedValue = randomizedValue.ToString("0.000000", CultureInfo.InvariantCulture);
        AkSoundEngine.SetRTPCValue(data.Parameter, float.Parse(formattedValue, CultureInfo.InvariantCulture));
    }

    private void CheckInactiveObjects()
    {
        foreach (WwiseInstanceDataObject data in instanceDataArray)
        {
            foreach (GameObject targetObject in data.targetObjects)
            {
                if (!targetObject.activeInHierarchy)
                {
                    targetObject.SetActive(true);
                    enterFlags[targetObject.name] = false;
                    instanceFlags[targetObject.name] = false;
                    targetObject.SetActive(false);
                }
            }
        }
    }
}

[System.Serializable]
public class CSVDataInstance
{
    public string Volume;
    public string Parameter;
    public float Value;
    public float MinRandomRange;
    public float MaxRandomRange;
}

[System.Serializable]
public class WwiseInstanceDataObject
{
    public List<GameObject> targetObjects;
    public AK.Wwise.Event instantiateEvent;
    public AK.Wwise.Event destroyEvent;
}
