using System.Collections;
using UnityEngine;

public class ActivateAfterTime : MonoBehaviour
{

    public float timeToActivate = 5.0f;
    public GameObject objectToActivate;

    void Start()
    {if (timeToActivate > 0){ StartCoroutine(ActivateObjectAfterTime());}}

    IEnumerator ActivateObjectAfterTime()
    {
       yield return new WaitForSeconds(timeToActivate);
       objectToActivate.SetActive(true);
    }
}
