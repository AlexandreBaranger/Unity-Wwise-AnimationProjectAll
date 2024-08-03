using UnityEngine;

public class QuitOnEscape : MonoBehaviour
{
    void Update()
    {
        // Vérifier si la touche Escape est enfoncée
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Quitter l'application
            Application.Quit();
        }
    }
}
