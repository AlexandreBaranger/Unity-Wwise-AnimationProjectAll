using UnityEngine;
using AK.Wwise;

public class RTPCAutoSet : MonoBehaviour
{
    public AK.Wwise.RTPC rtpc;
    public Collider triggerCollider;
    public float startValue = -96f;
    public float endValue = 0f;
    public float transitionTime = 1f;
    public AnimationCurve transitionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Range(-96f, 0f)]
    public float manualValue;

    private bool triggered = false;
    private bool exitTriggered = false; // Variable pour savoir si le trigger de sortie a déjà été déclenché
    private float transitionTimer = 0f;
    private float currentValue;

    void Start()
    {
        currentValue = startValue;
        rtpc.SetGlobalValue(currentValue);
    }

    void Update()
    {
        if (triggered)
        {
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / transitionTime);
            currentValue = Mathf.Lerp(startValue, endValue, transitionCurve.Evaluate(t));
            rtpc.SetGlobalValue(currentValue);
            manualValue = currentValue;

            if (transitionTimer >= transitionTime)
            {
                triggered = false;
                transitionTimer = 0f;
            }
        }
        else if (exitTriggered)
        {
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / transitionTime);
            currentValue = Mathf.Lerp(endValue, startValue, transitionCurve.Evaluate(t));
            rtpc.SetGlobalValue(currentValue);
            manualValue = currentValue;

            if (transitionTimer >= transitionTime)
            {
                exitTriggered = false; // Réinitialiser le déclencheur de sortie
                transitionTimer = 0f;
            }
        }
        else
        {
            rtpc.SetGlobalValue(manualValue);
            currentValue = manualValue;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!exitTriggered) // Si le trigger de sortie n'a pas été déclenché, on peut déclencher le fade-in
            {
                triggered = true;
                Debug.Log("Collision détectée avec succès. Déclencher le fade-in.");
            }
            else // Si le trigger de sortie a été déclenché, réinitialiser la variable exitTriggered
            {
                exitTriggered = false;
                Debug.Log("Collision détectée avec succès. Réinitialiser le déclencheur de sortie.");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!triggered) // Si le fade-in n'est pas en cours, déclencher le fade-out
            {
                exitTriggered = true;
                Debug.Log("Collision sortie avec succès. Déclencher le fade-out.");
            }
        }
    }
}
