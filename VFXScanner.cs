using System.IO;
using UnityEngine;

public class VFXScanner : MonoBehaviour
{
    public string logFilePath = "Assets/Audio/VFXLog.txt"; // Chemin du fichier log par défaut

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
        File.WriteAllText(logFilePath, "VFX Log\n\n");

        // Afficher le chemin du fichier dans la console
        Debug.Log("Le fichier texte est enregistré à : " + logFilePath);

        // Scanner la scène pour trouver tous les ParticleSystem
        ScanForParticleSystems();
    }

    private void ScanForParticleSystems()
    {
        ParticleSystem[] allParticleSystems = FindObjectsOfType<ParticleSystem>();
        Debug.Log("Nombre de ParticleSystems trouvés : " + allParticleSystems.Length);
        foreach (ParticleSystem ps in allParticleSystems)
        {
            GameObject go = ps.gameObject;
            Debug.Log("Found ParticleSystem: " + ps.name + " on GameObject: " + go.name);
            ParticleLogger logger = go.AddComponent<ParticleLogger>();
            logger.Initialize(ps, this);
        }
    }

    public void LogParticle(string particleName, float time, string gameObjectName)
    {
        string logEntry = string.Format("ParticleSystem: {0}, Time: {1}, GameObject: {2}\n", particleName, time, gameObjectName);
        File.AppendAllText(logFilePath, logEntry);
    }
}
