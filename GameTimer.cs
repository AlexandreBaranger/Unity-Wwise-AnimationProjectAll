using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Dur�e totale du jeu en secondes")]
    public float totalGameDuration = 60f;

    [Header("Activation / D�sactivation des GameObjects")]
    [Tooltip("Liste des activations et d�sactivations des GameObjects")]
    public List<GameObjectActivation> gameObjectActivations;

    public float gameTimer;
    public bool isGameRunning = false;
    public Image sliderImageComponent;
    public int currentImageIndex = 0;

    void Start()
    {
        gameTimer = 0f; // Commencer le timer du jeu � 0
        isGameRunning = true;

    }

    void Update()
    {
        if (isGameRunning)
        {
            gameTimer += Time.deltaTime; // Incr�menter le timer du jeu

            // V�rifier si le jeu a atteint la dur�e totale sp�cifi�e
            if (gameTimer >= totalGameDuration)
            {
                isGameRunning = false;
                StopAllProcesses();
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif

            }

            // V�rifier et activer/d�sactiver les GameObjects aux temps sp�cifi�s
            foreach (var activation in gameObjectActivations)
            {
                if (gameTimer >= activation.activationTime && !activation.hasActivated)
                {
                    StartCoroutine(ActivateGameObject(activation.gameObject));
                    activation.hasActivated = true;
                }
                if (gameTimer >= activation.deactivationTime && !activation.hasDeactivated)
                {
                    StartCoroutine(DeactivateGameObject(activation.gameObject));
                    activation.hasDeactivated = true;
                }
            }

        }
    }

    void StopAllProcesses()
    {
        // Arr�ter tous les coroutines en cours
        StopAllCoroutines();
    }



    private IEnumerator ActivateGameObject(GameObject gameObject)
    {
        yield return null; // Ajoutez des d�lais ou des conditions suppl�mentaires si n�cessaire
        gameObject.SetActive(true);
    }

    private IEnumerator DeactivateGameObject(GameObject gameObject)
    {
        yield return null; // Ajoutez des d�lais ou des conditions suppl�mentaires si n�cessaire
        gameObject.SetActive(false);
    }



    [System.Serializable]
    public class GameObjectActivation
    {
        public GameObject gameObject;
        public float activationTime;
        public float deactivationTime;
        [HideInInspector]
        public bool hasActivated = false;
        [HideInInspector]
        public bool hasDeactivated = false;
    }
}