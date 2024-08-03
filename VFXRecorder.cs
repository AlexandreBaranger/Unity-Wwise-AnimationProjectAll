using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VFXRecorder : MonoBehaviour
{
    public static VFXRecorder Instance;

    private string filePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            filePath = Path.Combine(Application.persistentDataPath, "VFXLog.txt");
            // Créer ou vider le fichier au démarrage
            File.WriteAllText(filePath, "VFX Log\n\n");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RecordVFX(string name, float time)
    {
        string logEntry = string.Format("VFX: {0}, Time: {1}\n", name, time);
        File.AppendAllText(filePath, logEntry);
    }
}
