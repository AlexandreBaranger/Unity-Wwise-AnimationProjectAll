using UnityEngine;
using UnityEngine.InputSystem;

public class AudioActionKeyboardToggle : MonoBehaviour
{
    public AK.Wwise.Event EventNumPad0First;
    public AK.Wwise.Event EventNumPad0Second;
    private InputAction numpad0Action;
    private bool toggleState = false;

    private void Awake()
    {
        numpad0Action = new InputAction(binding: "<Keyboard>/numpad0");
        numpad0Action.started += ctx => ToggleEvent();
    }

    private void OnEnable()
    {
        toggleState = false; // Initialiser l'état à false lors de l'activation
        numpad0Action.Enable();
    }

    private void OnDisable()
    {
        numpad0Action.Disable();
    }

    private void ToggleEvent()
    {
        if (toggleState)
        {
            AkSoundEngine.PostEvent(EventNumPad0First.Id, gameObject);
        }
        else
        {
            AkSoundEngine.PostEvent(EventNumPad0Second.Id, gameObject);
        }
        toggleState = !toggleState;
    }

    // Méthode pour déclencher le premier événement Wwise
    [ContextMenu("Play")]
    public void PlayEvent()
    {
        AkSoundEngine.PostEvent(EventNumPad0First.Id, gameObject);
    }
    [ContextMenu("Stop")]
    public void StopEvent()
    {
        AkSoundEngine.PostEvent(EventNumPad0Second.Id, gameObject);
    }
}
