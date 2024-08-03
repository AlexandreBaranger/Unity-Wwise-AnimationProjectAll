using System.IO;
using UnityEngine;

public class VFXScanner : MonoBehaviour
{
    public string logFilePath = "Assets/Audio/VFXLog.txt"; // Chemin du fichier log par d�faut

    private void Start()
    {
        // Utiliser le chemin sp�cifi� dans l'Inspector
        if (string.IsNullOrEmpty(logFilePath))
        {
            Debug.LogError("Le chemin du fichier log est vide ou non sp�cifi�.");
            return;
        }

        // S'assurer que le dossier existe
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

        // Cr�er ou vider le fichier au d�marrage
        File.WriteAllText(logFilePath, "VFX Log\n\n");

        // Afficher le chemin du fichier dans la console
        Debug.Log("Le fichier texte est enregistr� � : " + logFilePath);

        // Scanner la sc�ne pour trouver tous les ParticleSystem
        ScanForParticleSystems();
    }

    private void ScanForParticleSystems()
    {
        ParticleSystem[] allParticleSystems = FindObjectsOfType<ParticleSystem>();
        Debug.Log("Nombre de ParticleSystems trouv�s : " + allParticleSystems.Length);
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
