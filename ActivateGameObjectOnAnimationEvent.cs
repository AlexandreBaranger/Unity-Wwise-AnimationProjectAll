using UnityEngine;

public class ActivateGameObjectOnAnimationEvent : MonoBehaviour
{
    public GameObject objectToActivate; // GameObject à activer

    public void ActivateObject()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
        else
        {
            Debug.LogWarning("ObjectToActivate n'est pas assigné.");
        }
    }
}
