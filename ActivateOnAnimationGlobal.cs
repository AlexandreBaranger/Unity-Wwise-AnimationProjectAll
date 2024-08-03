using UnityEngine;

public class ActivateOnAnimationGlobal : MonoBehaviour
{
    public AnimationClip animationClip; // AnimationClip à surveiller
    public GameObject objectToActivate; // GameObject à activer
    public float refreshRate = 1.0f; // Taux de rafraîchissement en secondes
    public float startDelay = 0.0f; // Délai avant le début du scan

    public bool animationStarted = false; // Variable rendue publique pour débogage

    private float nextCheckTime = 0.0f; // Temps du prochain scan

    void Start()
    {
        // Initialiser le temps du premier scan
        nextCheckTime = Time.time + startDelay;
    }

    void Update()
    {
        // Vérifier si c'est le moment de faire un scan
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
            Debug.LogWarning("AnimationClip ou ObjectToActivate n'est pas assigné.");
            return;
        }

        bool foundAnimationPlaying = false;

        // Trouve tous les Animators dans la scène
        Animator[] animators = FindObjectsOfType<Animator>();

        // Vérifie chaque Animator pour voir s'il joue l'animation spécifiée
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
                        // L'animation a commencé
                        Debug.Log("Animation commencée: " + animationClip.name);
                        objectToActivate.SetActive(true);
                        animationStarted = true;
                    }
                    break;
                }
            }
            if (foundAnimationPlaying) break;
        }

        // Si l'animation n'est pas trouvée parmi les clips en cours, elle est terminée ou n'a pas encore commencé
        if (!foundAnimationPlaying && animationStarted)
        {
            Debug.Log("Animation terminée: " + animationClip.name);
            objectToActivate.SetActive(false);
            animationStarted = false;
        }
    }
}
