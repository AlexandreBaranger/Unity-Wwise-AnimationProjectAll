using UnityEngine;
using AK.Wwise;

public class KeyPad02WwiseEventTriggerAutoCurve : MonoBehaviour
{
    [Tooltip("Nombre d'événements à déclencher.")]
    public int numberOfEvents = 20;

    [Tooltip("Courbe utilisée pour répartir les événements dans le temps.")]
    public AnimationCurve timeCurve;

    [Tooltip("Durée totale pour déclencher les événements.")]
    public float duration = 20f;

    [Tooltip("Événement Wwise à déclencher.")]
    public AK.Wwise.Event wwiseEvent;

    private float[] eventTimes;
    private int nextEventIndex = 0;
    private float elapsedTime = 0f;

    void Start()
    {
        CalculateEventTimes();
    }

    void Update()
    {
        // Vérifie si la touche 2 du clavier numérique est pressée pour réinitialiser le script
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            nextEventIndex = 0;
            elapsedTime = 0f;
        }

        elapsedTime += Time.deltaTime;

        while (nextEventIndex < eventTimes.Length && elapsedTime >= eventTimes[nextEventIndex])
        {
            TriggerWwiseEvent();
            nextEventIndex++;
        }
    }

    void CalculateEventTimes()
    {
        eventTimes = new float[numberOfEvents];
        float totalCurveTime = 0f;

        for (int i = 0; i < numberOfEvents; i++)
        {
            float t = (float)i / (numberOfEvents - 1);
            // Inverse le t pour que les petits t viennent en premier
            float inverseT = 1f - t;
            float curveValue = timeCurve.Evaluate(inverseT);
            eventTimes[i] = totalCurveTime + (curveValue * duration / (numberOfEvents - 1));
            totalCurveTime = eventTimes[i];
        }
    }

    void TriggerWwiseEvent()
    {
        if (wwiseEvent != null)
        {
            wwiseEvent.Post(gameObject);
        }
        else
        {
            Debug.LogWarning("Aucun événement Wwise assigné.");
        }
    }
}
