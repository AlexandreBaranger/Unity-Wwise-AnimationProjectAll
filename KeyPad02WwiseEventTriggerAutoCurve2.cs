using UnityEngine;
using AK.Wwise;

public class KeyPad02WwiseEventTriggerAutoCurve2 : MonoBehaviour
{
    [Tooltip("Nombre d'�v�nements � d�clencher.")]
    public int numberOfEvents = 20;

    [Tooltip("Courbe utilis�e pour r�partir les �v�nements dans le temps.")]
    public AnimationCurve timeCurve;

    [Tooltip("Dur�e totale pour d�clencher les �v�nements.")]
    public float duration = 20f;

    [Tooltip("�v�nement Wwise � d�clencher.")]
    public AK.Wwise.Event wwiseEvent;

    [Tooltip("Deuxi�me �v�nement Wwise � d�clencher.")]
    public AK.Wwise.Event secondWwiseEvent;

    [Tooltip("D�lai entre le premier et le deuxi�me �v�nement en secondes.")]
    public float delayBetweenEvents = 1f;

    private float[] eventTimes;
    private int nextEventIndex = 0;
    private float elapsedTime = 0f;

    void Start()
    {
        CalculateEventTimes();
    }

    void Update()
    {
        // V�rifie si la touche 2 du clavier num�rique est press�e pour r�initialiser le script
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            TriggerWwiseEvent();
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
            // D�clenche le deuxi�me �v�nement apr�s le d�lai d�fini
            if (secondWwiseEvent != null)
            {
                StartCoroutine(TriggerSecondWwiseEventWithDelay());
            }
        }
        else
        {
            Debug.LogWarning("Aucun �v�nement Wwise assign�.");
        }
    }

    private System.Collections.IEnumerator TriggerSecondWwiseEventWithDelay()
    {
        yield return new WaitForSeconds(delayBetweenEvents);
        if (secondWwiseEvent != null)
        {
            secondWwiseEvent.Post(gameObject);
        }
    }
}
