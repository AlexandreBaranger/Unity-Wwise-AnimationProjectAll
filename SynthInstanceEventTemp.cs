using UnityEngine;
using System.Collections.Generic;

public class SynthInstaceEventTemp : MonoBehaviour
{
    public GameObject gameObjectToSync;
    public bool debugMode = false; // Ajouter une case à cocher pour le mode debug
    private Vector3 previousPosition;
    public WwiseInstanceDataObject[] instanceDataArray;
    public float sphereRadius = 1.0f;
    private Dictionary<string, bool> instanceFlags = new Dictionary<string, bool>();
    private Dictionary<string, GameObject> instances = new Dictionary<string, GameObject>();
    private Dictionary<string, bool> enterFlags = new Dictionary<string, bool>();

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
            List<string> objectsToRemove = new List<string>();

            foreach (WwiseInstanceDataObject data in instanceDataArray)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, sphereRadius);
                bool found = false;

                foreach (GameObject targetObject in data.targetObjects)
                {
                    foreach (Collider collider in colliders)
                    {
                        GameObject obj = collider.gameObject;
                        if (obj == targetObject)
                        {
                            found = true;
                            if (!enterFlags.ContainsKey(targetObject.name) || !enterFlags[targetObject.name])
                            {
                                if (!instanceFlags.ContainsKey(targetObject.name) || !instanceFlags[targetObject.name])
                                {
                                    Debug.Log("Objet trouvé : " + targetObject.name);
                                    Debug.Log("Coordonnées du GameObject trouvé : " + obj.transform.position);
                                    GameObject instance = Instantiate(targetObject, obj.transform.position, Quaternion.identity);
                                    instances[targetObject.name] = instance;
                                    instanceFlags[targetObject.name] = true;

                                    if (data.instantiateEvent != null)
                                    {
                                        data.instantiateEvent.Post(instance);
                                    }

                                    if (debugMode)
                                    {
                                        // Créer une sphère de debug
                                        GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                        debugSphere.transform.position = instance.transform.position;
                                        debugSphere.transform.localScale = Vector3.one * sphereRadius;
                                        debugSphere.GetComponent<Renderer>().material.color = Color.gray;
                                       // Destroy(debugSphere, 2.0f); // Détruire la sphère après 2 secondes
                                    }
                                }
                            }
                            enterFlags[targetObject.name] = true;
                        }
                    }
                }

                foreach (GameObject targetObject in data.targetObjects)
                {
                    if (!found)
                    {
                        enterFlags[targetObject.name] = false;
                        if (instanceFlags.ContainsKey(targetObject.name) && instanceFlags[targetObject.name])
                        {
                            instanceFlags[targetObject.name] = false;
                            if (instances.ContainsKey(targetObject.name))
                            {
                                Destroy(instances[targetObject.name]);
                                instances.Remove(targetObject.name);
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
    }
}

/*
[System.Serializable]
public class WwiseInstanceDataObject
{
    public List<GameObject> targetObjects;
    public AK.Wwise.Event instantiateEvent;
    public AK.Wwise.Event destroyEvent;
}*/
