using UnityEngine;

public class QuitOnEscape : MonoBehaviour
{
    void Update()
    {
        // V�rifier si la touche Escape est enfonc�e
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Quitter l'application
            Application.Quit();
        }
    }
}
