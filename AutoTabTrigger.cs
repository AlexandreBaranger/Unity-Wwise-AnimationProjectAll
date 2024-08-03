using UnityEngine;
using System.Collections;

public class AutoTabTrigger : MonoBehaviour
{
    // Temps avant de d�clencher la touche TAB (en secondes)
    public float delayTime = 5f;

    private float timer = 0f;
    private bool tabTriggered = false;

    // Update is called once per frame
    void Update()
    {
        // Incr�menter le compteur de temps
        timer += Time.deltaTime;

        // V�rifier si le temps �coul� est sup�rieur au temps de retard et si la touche TAB n'a pas encore �t� d�clench�e
        if (timer >= delayTime && !tabTriggered)
        {
            // Simuler la pression de la touche TAB
            StartCoroutine(SimulateTabPress());
            // Mettre � jour le bool�en pour indiquer que la touche TAB a �t� d�clench�e
            tabTriggered = true;
        }
    }

    IEnumerator SimulateTabPress()
    {
        // Attendre un court instant pour simuler la pression de la touche TAB
        yield return new WaitForSeconds(0.1f);
        // D�clencher la touche TAB
        Debug.Log("Touche TAB d�clench�e automatiquement !");
        // Utiliser la fonction d'Unity pour simuler la pression de la touche TAB
        KeyDown(KeyCode.Tab);
        KeyUp(KeyCode.Tab);
    }

    // Simuler la pression de la touche
    void KeyDown(KeyCode key)
    {
        if (Input.GetKeyDown(key))
            Debug.Log("KeyDown: " + key);
    }

    // Simuler le rel�chement de la touche
    void KeyUp(KeyCode key)
    {
        if (Input.GetKeyUp(key))
            Debug.Log("KeyUp: " + key);
    }
}
