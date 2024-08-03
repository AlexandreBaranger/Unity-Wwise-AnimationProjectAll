using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimerSliderText : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Dur�e totale du jeu en secondes")]
    public float totalGameDuration = 60f;

    [Header("Text Settings")]
    [Tooltip("Configuration des textes � afficher")]
    public List<TextConfig> sliderTexts;

    [Header("Activation / D�sactivation des GameObjects")]
    [Tooltip("Liste des activations et d�sactivations des GameObjects")]
    public List<GameObjectActivation> gameObjectActivations;

    public float gameTimer;
    public bool isGameRunning = false;
    public Text sliderTextComponent;
    public int currentTextIndex = 0;

    void Start()
    {
        gameTimer = 0f; // Commencer le timer du jeu � 0
        isGameRunning = true;

        // Cr�er le texte du slider en plein �cran
        GameObject textGO = new GameObject("SliderText");
        textGO.transform.SetParent(this.transform);
        Canvas canvas = textGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        textGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        textGO.AddComponent<GraphicRaycaster>();
        sliderTextComponent = textGO.AddComponent<Text>();

        // Ajuster RectTransform pour couvrir l'�cran complet
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

            // V�rifier et afficher les textes aux temps sp�cifi�s
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

    private IEnumerator DisplayText(TextConfig config)
    {
        while (currentTextIndex < sliderTexts.Count)
        {
            sliderTextComponent.gameObject.SetActive(true);
            var currentConfig = sliderTexts[currentTextIndex];
            sliderTextComponent.text = currentConfig.text;
            sliderTextComponent.fontSize = currentConfig.fontSize;
            sliderTextComponent.color = currentConfig.color;

            // Ajuster RectTransform en fonction des param�tres de TextConfig
            RectTransform rt = sliderTextComponent.GetComponent<RectTransform>();
            rt.sizeDelta = currentConfig.size;
            rt.anchoredPosition = currentConfig.position;

            // Fondu au noir
            yield return StartCoroutine(FadeIn(currentConfig.fadeInDuration));

            // Afficher le texte pour la dur�e sp�cifi�e sans fade
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
        public Vector2 size = new Vector2(1009, 278); // Taille par d�faut
        public float fadeInDuration = 1f; // Dur�e du fondu en entr�e
        public float fadeOutDuration = 1f; // Dur�e du fondu en sortie
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
