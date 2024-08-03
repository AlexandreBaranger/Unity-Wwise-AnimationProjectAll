using UnityEngine;

public class AnimationLogger : MonoBehaviour
{
    public Animator animator;       // Référence à l'Animator
    public AnimationScanner scanner; // Référence à l'AnimationScanner
    public string gameObjectName;   // Nom du GameObject
    private bool isAnimationPlaying = false;

    public void Initialize(Animator animatorComponent, AnimationScanner animationScanner)
    {
        animator = animatorComponent;
        scanner = animationScanner;
        gameObjectName = animator.gameObject.name;

        if (animator == null)
        {
            Debug.LogError("AnimationLogger: Animator is not assigned in Initialize.");
        }
    }

    private void Start()
    {
        if (animator == null)
        {
            Debug.LogError("AnimationLogger: Animator is not assigned in Start.");
        }
        else
        {
            Debug.Log("AnimationLogger initialized for Animator on GameObject: " + gameObjectName);
        }
    }

    private void Update()
    {
        if (animator == null)
        {
            return;  // Exit if Animator is not assigned
        }

        // Vérifier si une animation est en cours
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool && animator.GetBool(param.name))
            {
                LogAnimation(param.name, Time.time, gameObjectName);
            }
            else if (param.type == AnimatorControllerParameterType.Trigger && animator.GetBool(param.name))
            {
                LogAnimation(param.name, Time.time, gameObjectName);
            }
        }
    }

    private void LogAnimation(string animationName, float time, string gameObjectName)
    {
        if (!isAnimationPlaying)
        {
            isAnimationPlaying = true;
            scanner.LogAnimation(animationName, time, gameObjectName);
        }
    }
}
