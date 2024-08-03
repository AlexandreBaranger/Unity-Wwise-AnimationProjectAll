using UnityEngine;
using UnityEngine.InputSystem;
public class KeyboardNumpad1SynthLoadPreset : MonoBehaviour
{
    public AK.Wwise.Event EventNumPad;
    private InputAction numpadAction;

    private void Awake()
    {
        numpadAction = new InputAction(binding: "<Keyboard>/numpad1");
        numpadAction.started += ctx => { AkSoundEngine.PostEvent(EventNumPad.Id, gameObject); };
    }
    private void OnEnable()
    { numpadAction.Enable(); }
    private void OnDisable()
    { numpadAction.Disable(); }
}
