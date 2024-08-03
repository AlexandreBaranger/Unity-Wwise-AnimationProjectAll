using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimerSlider : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Durée totale du jeu en secondes")]
    public float totalGameDuration = 60f;

    [Header("Slider Settings")]
    [Tooltip("Configuration des images à afficher")]
    public List<ImageConfig> sliderImages;

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

        // Créer l'image du slider en plein écran
        GameObject sliderGO = new GameObject("SliderImage");
        sliderGO.transform.SetParent(this.transform);
        Canvas canvas = sliderGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        sliderGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        sliderGO.AddComponent<GraphicRaycaster>();
        sliderImageComponent = sliderGO.AddComponent<Image>();

        // Ajuster RectTransform pour couvrir l'écran complet
        RectTransform rt = sliderImageComponent.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(0, 0);

        sliderImageComponent.gameObject.SetActive(true);
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

            // Vérifier et afficher les images aux temps spécifiés
            foreach (var imageConfig in sliderImages)
            {
                if (gameTimer >= imageConfig.displayStartTime && !imageConfig.hasDisplayed)
                {
                    StartCoroutine(DisplayImage(imageConfig));
                    imageConfig.hasDisplayed = true;
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

    private IEnumerator DisplayImage(ImageConfig config)
    {
        while (currentImageIndex < sliderImages.Count)
        {
            sliderImageComponent.gameObject.SetActive(true);
            var currentConfig = sliderImages[currentImageIndex];
            sliderImageComponent.sprite = currentConfig.image;

            // Ajuster RectTransform en fonction des paramètres de ImageConfig
            RectTransform rt = sliderImageComponent.GetComponent<RectTransform>();
            rt.sizeDelta = currentConfig.size;
            rt.anchoredPosition = currentConfig.position;

            // Fondu au noir
            yield return StartCoroutine(FadeIn(currentConfig.fadeInDuration));

            // Afficher l'image pour la durée spécifiée sans fade
            yield return new WaitForSeconds(currentConfig.displayDuration);

            // Fondu au clair
            yield return StartCoroutine(FadeOut(currentConfig.fadeOutDuration));

            currentImageIndex++;
        }
    }

    private IEnumerator FadeIn(float duration)
    {
        Color color = sliderImageComponent.color;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(0f, 1f, t / duration);
            sliderImageComponent.color = color;
            yield return null;
        }
        color.a = 1f;
        sliderImageComponent.color = color;
    }

    private IEnumerator FadeOut(float duration)
    {
        Color color = sliderImageComponent.color;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(1f, 0f, t / duration);
            sliderImageComponent.color = color;
            yield return null;
        }
        color.a = 0f;
        sliderImageComponent.color = color;
    }

    [System.Serializable]
    public class ImageConfig
    {
        public Sprite image;
        public float displayStartTime = 0f;
        public float displayDuration = 1f;
        public Vector2 position = Vector2.zero;
        public Vector2 size = new Vector2(1009, 278); // Taille par défaut
        public float fadeInDuration = 1f; // Durée du fondu en entrée
        public float fadeOutDuration = 1f; // Durée du fondu en sortie
        [HideInInspector]
        public bool hasDisplayed = false;
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
