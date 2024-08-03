using UnityEngine;

public class ActivateOnAnimation : MonoBehaviour
{
    public Animator animator; // R�f�rence � l'Animator
    public AnimationClip animationClip; // AnimationClip � surveiller
    public GameObject objectToActivate; // GameObject � activer
    public GameObject animationPrefab; // Prefab contenant l'animation

    public bool animationStarted = false; // Variable rendue publique pour d�bogage

    private Animator prefabAnimator;

    void Start()
    {
        if (animationPrefab == null)
        {
            Debug.LogWarning("Le prefab d'animation n'est pas assign�.");
            return;
        }

        // Instancier le prefab et trouver son Animator
        GameObject instantiatedPrefab = Instantiate(animationPrefab);
        prefabAnimator = instantiatedPrefab.GetComponentInChildren<Animator>();

        if (prefabAnimator == null)
        {
            Debug.LogWarning("Aucun Animator trouv� dans le prefab d'animation.");
        }
    }

    void Update()
    {
        if (prefabAnimator == null || animationClip == null || objectToActivate == null)
        {
            return;
        }

        // V�rifie si l'animation sp�cifi�e est en cours d'ex�cution
        AnimatorClipInfo[] currentClipInfo = prefabAnimator.GetCurrentAnimatorClipInfo(0);
        foreach (var clipInfo in currentClipInfo)
        {
            if (clipInfo.clip == animationClip)
            {
                if (!animationStarted)
                {
                    // L'animation a commenc�
                    Debug.Log("Animation commenc�e: " + animationClip.name);
                    objectToActivate.SetActive(true);
                    animationStarted = true;
                }
                return;
            }
        }

        // Si l'animation n'est pas trouv�e parmi les clips en cours, elle est termin�e ou n'a pas encore commenc�
        if (animationStarted)
        {
            Debug.Log("Animation termin�e: " + animationClip.name);
            objectToActivate.SetActive(false);
            animationStarted = false;
        }
    }
}
