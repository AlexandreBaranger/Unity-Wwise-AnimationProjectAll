using UnityEngine;
using AK.Wwise;

public class KeyPad01WwiseEventTriggerAutoTime : MonoBehaviour
{
    // Options pour d�clencher l'�v�nement
    public bool triggerByFrequency = false;

    // Param�tres pour le d�clenchement par fr�quence
    [Tooltip("Fr�quence en secondes pour d�clencher l'�v�nement")]
    public float triggerFrequency = 1.0f; // Fr�quence de d�clenchement en secondes

    // L'�v�nement Wwise � d�clencher
    public AK.Wwise.Event wwiseEvent;

    // Timer pour g�rer le d�clenchement par fr�quence
    private float timer = 0.0f;
    private bool scriptEnabled = true; // Ajout d'une variable pour activer/d�sactiver le script

    void Update()
    {
        // V�rifie si la touche 3 du clavier num�rique est press�e pour activer ou d�sactiver le script
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            scriptEnabled = !scriptEnabled;
            timer = 0.0f; // R�initialiser le timer si le script est d�sactiv�
        }

        if (!scriptEnabled)
            return; // Si le script est d�sactiv�, sortir de la fonction Update()

        if (triggerByFrequency)
        {
            timer += Time.deltaTime;

            if (timer >= triggerFrequency)
            {
                // D�clenchement de l'�v�nement Wwise
                wwiseEvent.Post(gameObject);
                timer = 0.0f; // R�initialiser le timer
            }
        }
    }
}
