using UnityEngine;

public class ActivateOnAnimation : MonoBehaviour
{
    public Animator animator; // Référence à l'Animator
    public AnimationClip animationClip; // AnimationClip à surveiller
    public GameObject objectToActivate; // GameObject à activer
    public GameObject animationPrefab; // Prefab contenant l'animation

    public bool animationStarted = false; // Variable rendue publique pour débogage

    private Animator prefabAnimator;

    void Start()
    {
        if (animationPrefab == null)
        {
            Debug.LogWarning("Le prefab d'animation n'est pas assigné.");
            return;
        }

        // Instancier le prefab et trouver son Animator
        GameObject instantiatedPrefab = Instantiate(animationPrefab);
        prefabAnimator = instantiatedPrefab.GetComponentInChildren<Animator>();

        if (prefabAnimator == null)
        {
            Debug.LogWarning("Aucun Animator trouvé dans le prefab d'animation.");
        }
    }

    void Update()
    {
        if (prefabAnimator == null || animationClip == null || objectToActivate == null)
        {
            return;
        }

        // Vérifie si l'animation spécifiée est en cours d'exécution
        AnimatorClipInfo[] currentClipInfo = prefabAnimator.GetCurrentAnimatorClipInfo(0);
        foreach (var clipInfo in currentClipInfo)
        {
            if (clipInfo.clip == animationClip)
            {
                if (!animationStarted)
                {
                    // L'animation a commencé
                    Debug.Log("Animation commencée: " + animationClip.name);
                    objectToActivate.SetActive(true);
                    animationStarted = true;
                }
                return;
            }
        }

        // Si l'animation n'est pas trouvée parmi les clips en cours, elle est terminée ou n'a pas encore commencé
        if (animationStarted)
        {
            Debug.Log("Animation terminée: " + animationClip.name);
            objectToActivate.SetActive(false);
            animationStarted = false;
        }
    }
}
