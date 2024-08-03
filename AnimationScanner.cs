using System.IO;
using UnityEngine;

public class AnimationScanner : MonoBehaviour
{
    public string logFilePath = "Assets/AnimationsLog.txt"; // Chemin du fichier log par défaut

    private void Start()
    {
        // Utiliser le chemin spécifié dans l'Inspector
        if (string.IsNullOrEmpty(logFilePath))
        {
            Debug.LogError("Le chemin du fichier log est vide ou non spécifié.");
            return;
        }

        // S'assurer que le dossier existe
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

        // Créer ou vider le fichier au démarrage
        File.WriteAllText(logFilePath, "Animations Log\n\n");

        // Afficher le chemin du fichier dans la console
        Debug.Log("Le fichier texte est enregistré à : " + logFilePath);

        // Scanner la scène pour trouver tous les GameObjects avec Animator
        ScanForAnimators();
    }

    private void ScanForAnimators()
    {
        Animator[] allAnimators = FindObjectsOfType<Animator>();
        Debug.Log("Nombre d'Animator trouvés : " + allAnimators.Length);
        foreach (Animator animator in allAnimators)
        {
            GameObject go = animator.gameObject;
            Debug.Log("Found Animator: " + animator.name + " on GameObject: " + go.name);
            AnimationLogger logger = go.AddComponent<AnimationLogger>();
            logger.Initialize(animator, this);
        }
    }

    public void LogAnimation(string animationName, float time, string gameObjectName)
    {
        string logEntry = string.Format("Animation: {0}, Time: {1}, GameObject: {2}\n", animationName, time, gameObjectName);
        File.AppendAllText(logFilePath, logEntry);
    }
}
