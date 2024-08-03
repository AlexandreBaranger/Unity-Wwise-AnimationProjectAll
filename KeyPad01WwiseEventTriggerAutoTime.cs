using UnityEngine;
using AK.Wwise;

public class KeyPad01WwiseEventTriggerAutoTime : MonoBehaviour
{
    // Options pour déclencher l'événement
    public bool triggerByFrequency = false;

    // Paramètres pour le déclenchement par fréquence
    [Tooltip("Fréquence en secondes pour déclencher l'événement")]
    public float triggerFrequency = 1.0f; // Fréquence de déclenchement en secondes

    // L'événement Wwise à déclencher
    public AK.Wwise.Event wwiseEvent;

    // Timer pour gérer le déclenchement par fréquence
    private float timer = 0.0f;
    private bool scriptEnabled = true; // Ajout d'une variable pour activer/désactiver le script

    void Update()
    {
        // Vérifie si la touche 3 du clavier numérique est pressée pour activer ou désactiver le script
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            scriptEnabled = !scriptEnabled;
            timer = 0.0f; // Réinitialiser le timer si le script est désactivé
        }

        if (!scriptEnabled)
            return; // Si le script est désactivé, sortir de la fonction Update()

        if (triggerByFrequency)
        {
            timer += Time.deltaTime;

            if (timer >= triggerFrequency)
            {
                // Déclenchement de l'événement Wwise
                wwiseEvent.Post(gameObject);
                timer = 0.0f; // Réinitialiser le timer
            }
        }
    }
}
