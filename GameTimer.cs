using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Durée totale du jeu en secondes")]
    public float totalGameDuration = 60f;

    [Header("Activation / Désactivation des GameObjects")]
    [Tooltip("Liste des activations et désactivations des GameObjects")]
    public List<GameObjectActivation> gameObjectActivations;

    public float gameTimer;
    public bool isGameRunning = false;
    public Image sliderImageComponent;
    public int currentImageIndex = 0;

    void Start()
    {
        gameTimer = 0f; // Commencer le timer du jeu à 0
        isGameRunning = true;

    }

    void Update()
    {
        if (isGameRunning)
        {
            gameTimer += Time.deltaTime; // Incrémenter le timer du jeu

            // Vérifier si le jeu a atteint la durée totale spécifiée
            if (gameTimer >= totalGameDuration)
            {
                isGameRunning = false;
                StopAllProcesses();
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif

            }

            // Vérifier et activer/désactiver les GameObjects aux temps spécifiés
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
        // Arrêter tous les coroutines en cours
        StopAllCoroutines();
    }



    private IEnumerator ActivateGameObject(GameObject gameObject)
    {
        yield return null; // Ajoutez des délais ou des conditions supplémentaires si nécessaire
        gameObject.SetActive(true);
    }

    private IEnumerator DeactivateGameObject(GameObject gameObject)
    {
        yield return null; // Ajoutez des délais ou des conditions supplémentaires si nécessaire
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