using UnityEngine;
using System.Collections;

public class ChangeColorAfterDelay : MonoBehaviour
{
    public float delayInSeconds = 0f;
    public GameObject targetGameObject;
    public Color associatedColor;

    void Start()
    {
        if (targetGameObject != null)
        {
            // Assurez-vous que l'objet cible a un Renderer attaché
            Renderer objectRenderer = targetGameObject.GetComponent<Renderer>();
            if (objectRenderer != null)
            {
                StartCoroutine(ChangeColorWithDelay(objectRenderer));
            }
            else
            {
                Debug.LogWarning("Target GameObject does not have a Renderer component.");
            }
        }
        else
        {
            Debug.LogWarning("Target GameObject is not assigned.");
        }
    }

    IEnumerator ChangeColorWithDelay(Renderer objectRenderer)
    {
        yield return new WaitForSeconds(delayInSeconds);
        // Change la couleur de l'objet après le délai
        objectRenderer.material.color = associatedColor;
    }
}
