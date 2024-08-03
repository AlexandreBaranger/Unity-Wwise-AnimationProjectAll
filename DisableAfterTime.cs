using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class DisableAfterTime : MonoBehaviour
{
     public float disableTime = 5f;

    void Start()
    {
        if (disableTime > 0)
        {
            StartCoroutine(DisableAfterDelay());
        }
    }

    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(disableTime);
        gameObject.SetActive(false);
    }
}
