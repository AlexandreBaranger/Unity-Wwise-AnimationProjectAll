using UnityEngine;
using System.Collections.Generic;
using AK.Wwise;

public class WwiseStateController : MonoBehaviour
{
    public List<EventToggle> eventToggles = new List<EventToggle>();
    public GameObject targetGameObject;

    [System.Serializable]
    public class EventToggle
    {
        public AK.Wwise.Event EventPlayTab;
        public string stateGroupName;
        public AK.Wwise.Event stateEvent;
        public bool isStateActive;
        public Color associatedColor;
        public GameObject objectToActivate;
    }

    private Renderer objectRenderer;
    private int currentIndex = 0;

    private void Start()
    {
        InitializeRenderer();
        if (eventToggles.Count > 0)
        {
            UpdateWwiseStates();
        }
        else
        {
            Debug.LogWarning("La liste des eventToggles est vide.");
        }
    }

    private void InitializeRenderer()
    {
        if (targetGameObject != null)
        {
            objectRenderer = targetGameObject.GetComponent<Renderer>();
            if (objectRenderer == null)
            {
                Debug.LogWarning("Aucun Renderer trouvé sur le GameObject cible.");
            }
        }
        else
        {
            Debug.LogWarning("Aucun GameObject cible n'est défini dans le script.");
        }
    }

    private void UpdateWwiseStates()
    {
        foreach (var toggle in eventToggles)
        {
            if (toggle.isStateActive)
            {
                if (toggle.stateEvent != null)
                {
                    toggle.stateEvent.Post(gameObject);
                }
                else
                {
                    Debug.LogWarning($"StateEvent n'est pas défini pour {toggle.stateGroupName}");
                }

                if (objectRenderer != null)
                {
                    objectRenderer.material.color = toggle.associatedColor;
                }

                if (toggle.objectToActivate != null)
                {
                    toggle.objectToActivate.SetActive(true);
                }
                else
                {
                    Debug.LogWarning($"ObjectToActivate n'est pas défini pour {toggle.stateGroupName}");
                }
            }
            else
            {
                if (toggle.objectToActivate != null)
                {
                    toggle.objectToActivate.SetActive(false);
                }
            }
        }
    }

    private void Awake()
    {
        UpdateWwiseStates();
    }

    private void OnValidate()
    {
        UpdateWwiseStates();
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && eventToggles.Count > 0)
        {
            currentIndex = (currentIndex + 1) % eventToggles.Count;
            ToggleState(currentIndex);
            var currentToggle = eventToggles[currentIndex];
            if (currentToggle.EventPlayTab != null && currentToggle.EventPlayTab.Id != 0)
            {
                AkSoundEngine.PostEvent(currentToggle.EventPlayTab.Id, gameObject);
            }
            else
            {
                Debug.LogWarning($"EventPlayTab n'est pas défini ou l'ID est invalide pour {currentToggle.stateGroupName}");
            }
        }
    }

    private void ToggleState(int index)
    {
        for (int i = 0; i < eventToggles.Count; i++)
        {
            eventToggles[i].isStateActive = (i == index);
        }
        UpdateWwiseStates();
    }
}
