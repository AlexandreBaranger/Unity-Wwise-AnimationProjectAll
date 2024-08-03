using UnityEngine;

public class AudioColliderWwise : MonoBehaviour
{
    public bool useEnterEvent = false;
    public bool useStayEvent = false;
    public bool useExitEvent = false;
    public bool canTriggerOnce = false;
    public bool debugMode = false;
    private bool hasEntered = false;
    private bool hasTriggered = false;

    public AK.Wwise.Event enterEventName;
    public AK.Wwise.Event stayEventName;
    public AK.Wwise.Event exitEventName;
    public AudioEventLauncher[] eventLaunchers;
    private void OnTriggerEnter(Collider other)
    {
        if (canTriggerOnce && hasTriggered) return;
        if (useEnterEvent && other.CompareTag("Player") && !hasEntered)
        {
            foreach (var launcher in eventLaunchers)
            {launcher.LaunchEnterEvent();}
            if (debugMode) { Debug.Log("Enter Event Sent: " + enterEventName); }
            if (canTriggerOnce){hasTriggered = true;}
            hasEntered = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (canTriggerOnce && hasTriggered) return;
        if (useStayEvent && other.CompareTag("Player"))
        {
            foreach (var launcher in eventLaunchers)
            {launcher.LaunchStayEvent();}
            if (debugMode) { Debug.Log("Stay Event Sent: " + stayEventName); }
            if (canTriggerOnce){hasTriggered = true;}
            hasEntered = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (useExitEvent && other.CompareTag("Player"))
        {
            foreach (var launcher in eventLaunchers)
            {launcher.LaunchExitEvent();}
            if (debugMode) { Debug.Log("Exit Event Sent: " + exitEventName); }
            hasEntered = false;
            if (canTriggerOnce){hasTriggered = true;}
        }
    }
}
