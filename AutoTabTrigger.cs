using UnityEngine;
using System.Collections;

public class AutoTabTrigger : MonoBehaviour
{
    // Temps avant de déclencher la touche TAB (en secondes)
    public float delayTime = 5f;

    private float timer = 0f;
    private bool tabTriggered = false;

    // Update is called once per frame
    void Update()
    {
        // Incrémenter le compteur de temps
        timer += Time.deltaTime;

        // Vérifier si le temps écoulé est supérieur au temps de retard et si la touche TAB n'a pas encore été déclenchée
        if (timer >= delayTime && !tabTriggered)
        {
            // Simuler la pression de la touche TAB
            StartCoroutine(SimulateTabPress());
            // Mettre à jour le booléen pour indiquer que la touche TAB a été déclenchée
            tabTriggered = true;
        }
    }

    IEnumerator SimulateTabPress()
    {
        // Attendre un court instant pour simuler la pression de la touche TAB
        yield return new WaitForSeconds(0.1f);
        // Déclencher la touche TAB
        Debug.Log("Touche TAB déclenchée automatiquement !");
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

    // Simuler le relâchement de la touche
    void KeyUp(KeyCode key)
    {
        if (Input.GetKeyUp(key))
            Debug.Log("KeyUp: " + key);
    }
}
