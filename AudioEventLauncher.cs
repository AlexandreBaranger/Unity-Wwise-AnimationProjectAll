using UnityEngine;

public class AudioEventLauncher : MonoBehaviour
{
    public AK.Wwise.Event enterEvent;
    public AK.Wwise.Event stayEvent;
    public AK.Wwise.Event exitEvent;
    public void LaunchEnterEvent()
    {AkSoundEngine.PostEvent(enterEvent.Id, gameObject);}
    public void LaunchStayEvent()
    {AkSoundEngine.PostEvent(stayEvent.Id, gameObject);}
    public void LaunchExitEvent()
    {AkSoundEngine.PostEvent(exitEvent.Id, gameObject);}
}
