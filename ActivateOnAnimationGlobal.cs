using UnityEngine;

public class ActivateOnAnimationGlobal : MonoBehaviour
{
    public AnimationClip animationClip; // AnimationClip � surveiller
    public GameObject objectToActivate; // GameObject � activer
    public float refreshRate = 1.0f; // Taux de rafra�chissement en secondes
    public float startDelay = 0.0f; // D�lai avant le d�but du scan

    public bool animationStarted = false; // Variable rendue publique pour d�bogage

    private float nextCheckTime = 0.0f; // Temps du prochain scan

    void Start()
    {
        // Initialiser le temps du premier scan
        nextCheckTime = Time.time + startDelay;
    }

    void Update()
    {
        // V�rifier si c'est le moment de faire un scan
        if (Time.time >= nextCheckTime)
        {
            CheckAnimations();
            // Planifier le prochain scan
            nextCheckTime = Time.time + refreshRate;
        }
    }

    void CheckAnimations()
    {
        if (animationClip == null || objectToActivate == null)
        {
            Debug.LogWarning("AnimationClip ou ObjectToActivate n'est pas assign�.");
            return;
        }

        bool foundAnimationPlaying = false;

        // Trouve tous les Animators dans la sc�ne
        Animator[] animators = FindObjectsOfType<Animator>();

        // V�rifie chaque Animator pour voir s'il joue l'animation sp�cifi�e
        foreach (var animator in animators)
        {
            AnimatorClipInfo[] currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
            foreach (var clipInfo in currentClipInfo)
            {
                if (clipInfo.clip == animationClip)
                {
                    foundAnimationPlaying = true;
                    if (!animationStarted)
                    {
                        // L'animation a commenc�
                        Debug.Log("Animation commenc�e: " + animationClip.name);
                        objectToActivate.SetActive(true);
                        animationStarted = true;
                    }
                    break;
                }
            }
            if (foundAnimationPlaying) break;
        }

        // Si l'animation n'est pas trouv�e parmi les clips en cours, elle est termin�e ou n'a pas encore commenc�
        if (!foundAnimationPlaying && animationStarted)
        {
            Debug.Log("Animation termin�e: " + animationClip.name);
            objectToActivate.SetActive(false);
            animationStarted = false;
        }
    }
}
