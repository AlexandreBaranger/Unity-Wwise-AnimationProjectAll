using UnityEngine;

public class ParticleLogger : MonoBehaviour
{
    public ParticleSystem ps;       // Référence au ParticleSystem
    public VFXScanner scanner;      // Référence au VFXScanner
    public string particleName;     // Nom du ParticleSystem
    public string gameObjectName;   // Nom du GameObject
    private bool isParticleSystemPlaying = false;

    public void Initialize(ParticleSystem psComponent, VFXScanner vfxScanner)
    {
        ps = psComponent;
        scanner = vfxScanner;
        particleName = ps.name;
        gameObjectName = ps.gameObject.name;

        if (ps == null)
        {
            Debug.LogError("ParticleLogger: ParticleSystem is not assigned in Initialize.");
        }
    }

    private void Start()
    {
        if (ps == null)
        {
            Debug.LogError("ParticleLogger: ParticleSystem is not assigned in Start.");
        }
        else
        {
            Debug.Log("ParticleLogger initialized for ParticleSystem: " + particleName + " on GameObject: " + gameObjectName);
        }
    }

    private void Update()
    {
        if (ps == null)
        {
            return;  // Exit if ParticleSystem is not assigned
        }

        if (ps.isPlaying && !isParticleSystemPlaying)
        {
            isParticleSystemPlaying = true;
            float startTime = Time.time;
            scanner.LogParticle(particleName, startTime, gameObjectName);
        }
        else if (!ps.isPlaying && isParticleSystemPlaying)
        {
            isParticleSystemPlaying = false;
        }
    }
}
