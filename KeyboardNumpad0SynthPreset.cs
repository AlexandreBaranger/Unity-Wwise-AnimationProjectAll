using UnityEngine;
using UnityEngine.InputSystem;
public class KeyboardNumpad0SynthPreset : MonoBehaviour
{
    public AK.Wwise.Event EventNumPad0;
    private InputAction numpad0Action;

    private void Awake()
    {
        numpad0Action = new InputAction(binding: "<Keyboard>/numpad0");
        numpad0Action.started += ctx => { AkSoundEngine.PostEvent(EventNumPad0.Id, gameObject); };
    }
    private void OnEnable()
    { numpad0Action.Enable(); }
    private void OnDisable()
    { numpad0Action.Disable(); }
}
