using UnityEngine;

public class WwiseAddAnimationEvent : MonoBehaviour
{
    public AnimationClip clip;
    public AK.Wwise.Event eventWwise;
    public float timeAnimation;
   
    [WwiseAnimationSelectorAttribute(typeof(WwiseAddAnimationEvent))]
    public string methodName;

    void Start()
    {
        if (clip != null && !string.IsNullOrEmpty(methodName))
        {
            AnimationEvent evt = new AnimationEvent();
            evt.time = timeAnimation;
            evt.functionName = "InvokeMethod";
            evt.stringParameter = methodName;
            clip.AddEvent(evt);
        }
    }

    void InvokeMethod(string methodName)
    {
        var split = methodName.Split('.');
        if (split.Length == 2)
        {
            var className = split[0];
            var method = split[1];

            if (className == nameof(WwiseAddAnimationEvent))
            {
                Invoke(method, 0f);
            }
        }
    }

    public void OnAnimationEvent()
    {
        AkSoundEngine.PostEvent(eventWwise.Id, gameObject);
    }
}
