using UnityEngine;
using UnityEngine.InputSystem;
public class AudioActionKeyboard : MonoBehaviour
{
    public AK.Wwise.Event EventNumPad0;
    public AK.Wwise.Event EventNumPad1;
    public AK.Wwise.Event EventNumPad2;
    public AK.Wwise.Event EventNumPad3;
    public AK.Wwise.Event EventNumPad4;
    public AK.Wwise.Event EventNumPad5;
    public AK.Wwise.Event EventNumPad6;
    public AK.Wwise.Event EventNumPad7;
    public AK.Wwise.Event EventNumPad8;
    public AK.Wwise.Event EventNumPad9;

    private InputAction numpad0Action;
    private InputAction numpad1Action;
    private InputAction numpad2Action;
    private InputAction numpad3Action;
    private InputAction numpad4Action;
    private InputAction numpad5Action;
    private InputAction numpad6Action;
    private InputAction numpad7Action;
    private InputAction numpad8Action;
    private InputAction numpad9Action;

    private void Awake()
    {
        numpad0Action = new InputAction(binding: "<Keyboard>/numpad0");
        numpad0Action.started += ctx => {AkSoundEngine.PostEvent(EventNumPad0.Id, gameObject);};
        numpad1Action = new InputAction(binding: "<Keyboard>/numpad1");
        numpad1Action.started += ctx => {AkSoundEngine.PostEvent(EventNumPad1.Id, gameObject);};
        numpad2Action = new InputAction(binding: "<Keyboard>/numpad2");
        numpad2Action.started += ctx => {AkSoundEngine.PostEvent(EventNumPad2.Id, gameObject);};
        numpad3Action = new InputAction(binding: "<Keyboard>/numpad3");
        numpad3Action.started += ctx => {AkSoundEngine.PostEvent(EventNumPad3.Id, gameObject);};
        numpad4Action = new InputAction(binding: "<Keyboard>/numpad4");
        numpad4Action.started += ctx => {AkSoundEngine.PostEvent(EventNumPad4.Id, gameObject);};
        numpad5Action = new InputAction(binding: "<Keyboard>/numpad5");
        numpad5Action.started += ctx => {AkSoundEngine.PostEvent(EventNumPad5.Id, gameObject);};
        numpad6Action = new InputAction(binding: "<Keyboard>/numpad6");
        numpad6Action.started += ctx => {AkSoundEngine.PostEvent(EventNumPad6.Id, gameObject);};
        numpad7Action = new InputAction(binding: "<Keyboard>/numpad7");
        numpad7Action.started += ctx => {AkSoundEngine.PostEvent(EventNumPad7.Id, gameObject);};
        numpad8Action = new InputAction(binding: "<Keyboard>/numpad8");
        numpad8Action.started += ctx => {AkSoundEngine.PostEvent(EventNumPad8.Id, gameObject);};
        numpad9Action = new InputAction(binding: "<Keyboard>/numpad9");
        numpad9Action.started += ctx => {AkSoundEngine.PostEvent(EventNumPad9.Id, gameObject);};
    }
    private void OnEnable()
    {
        numpad0Action.Enable();
        numpad1Action.Enable();
        numpad2Action.Enable();
        numpad3Action.Enable();
        numpad4Action.Enable();
        numpad5Action.Enable();
        numpad6Action.Enable();
        numpad7Action.Enable();
        numpad8Action.Enable();
        numpad9Action.Enable();
    }
    private void OnDisable()
    {
        numpad0Action.Disable();
        numpad1Action.Disable();
        numpad2Action.Disable();
        numpad3Action.Disable();
        numpad4Action.Disable();
        numpad5Action.Disable();
        numpad6Action.Disable();
        numpad7Action.Disable();
        numpad8Action.Disable();
        numpad9Action.Disable();
    }
}
