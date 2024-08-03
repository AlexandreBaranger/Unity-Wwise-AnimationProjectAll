using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimerSliderText : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Durée totale du jeu en secondes")]
    public float totalGameDuration = 60f;

    [Header("Text Settings")]
    [Tooltip("Configuration des textes à afficher")]
    public List<TextConfig> sliderTexts;

    [Header("Activation / Désactivation des GameObjects")]
    [Tooltip("Liste des activations et désactivations des GameObjects")]
    public List<GameObjectActivation> gameObjectActivations;

    public float gameTimer;
    public bool isGameRunning = false;
    public Text sliderTextComponent;
    public int currentTextIndex = 0;

    void Start()
    {
        gameTimer = 0f; // Commencer le timer du jeu à 0
        isGameRunning = true;

        // Créer le texte du slider en plein écran
        GameObject textGO = new GameObject("SliderText");
        textGO.transform.SetParent(this.transform);
        Canvas canvas = textGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        textGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        textGO.AddComponent<GraphicRaycaster>();
        sliderTextComponent = textGO.AddComponent<Text>();

        // Ajuster RectTransform pour couvrir l'écran complet
        RectTransform rt = sliderTextComponent.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(0, 0);

        sliderTextComponent.alignment = TextAnchor.MiddleCenter;
        sliderTextComponent.gameObject.SetActive(true);
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

            // Vérifier et afficher les textes aux temps spécifiés
            foreach (var textConfig in sliderTexts)
            {
                if (gameTimer >= textConfig.displayStartTime && !textConfig.hasDisplayed)
                {
                    StartCoroutine(DisplayText(textConfig));
                    textConfig.hasDisplayed = true;
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

    private IEnumerator DisplayText(TextConfig config)
    {
        while (currentTextIndex < sliderTexts.Count)
        {
            sliderTextComponent.gameObject.SetActive(true);
            var currentConfig = sliderTexts[currentTextIndex];
            sliderTextComponent.text = currentConfig.text;
            sliderTextComponent.fontSize = currentConfig.fontSize;
            sliderTextComponent.color = currentConfig.color;

            // Ajuster RectTransform en fonction des paramètres de TextConfig
            RectTransform rt = sliderTextComponent.GetComponent<RectTransform>();
            rt.sizeDelta = currentConfig.size;
            rt.anchoredPosition = currentConfig.position;

            // Fondu au noir
            yield return StartCoroutine(FadeIn(currentConfig.fadeInDuration));

            // Afficher le texte pour la durée spécifiée sans fade
            yield return new WaitForSeconds(currentConfig.displayDuration);

            // Fondu au clair
            yield return StartCoroutine(FadeOut(currentConfig.fadeOutDuration));

            currentTextIndex++;
        }
    }

    private IEnumerator FadeIn(float duration)
    {
        Color color = sliderTextComponent.color;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(0f, 1f, t / duration);
            sliderTextComponent.color = color;
            yield return null;
        }
        color.a = 1f;
        sliderTextComponent.color = color;
    }

    private IEnumerator FadeOut(float duration)
    {
        Color color = sliderTextComponent.color;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(1f, 0f, t / duration);
            sliderTextComponent.color = color;
            yield return null;
        }
        color.a = 0f;
        sliderTextComponent.color = color;
    }

    [System.Serializable]
    public class TextConfig
    {
        public string text;
        public float displayStartTime = 0f;
        public float displayDuration = 1f;
        public Vector2 position = Vector2.zero;
        public Vector2 size = new Vector2(1009, 278); // Taille par défaut
        public float fadeInDuration = 1f; // Durée du fondu en entrée
        public float fadeOutDuration = 1f; // Durée du fondu en sortie
        public int fontSize = 14;
        public Color color = Color.white;
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
