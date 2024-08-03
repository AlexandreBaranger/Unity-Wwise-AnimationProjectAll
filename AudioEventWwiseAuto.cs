using UnityEngine;
using System.Collections;

public class AudioEventWwiseAuto : MonoBehaviour
{
    public AK.Wwise.Event wwiseEvent;
    public float delayInSeconds = 5f; 

    void Start()
    {
        if (delayInSeconds > 0)
        {
            StartCoroutine(TriggerEventWithDelay());
        }
    }

    IEnumerator TriggerEventWithDelay()
    {
        yield return new WaitForSeconds(delayInSeconds);
        wwiseEvent.Post(gameObject);
    }
}

