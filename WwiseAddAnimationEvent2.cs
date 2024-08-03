using UnityEngine;
using System;
using System.Collections.Generic;

public class WwiseAddAnimationEvent2 : MonoBehaviour
{
    [Serializable]
    public class AnimationEventData
    {
        public AnimationClip clip;
        public AK.Wwise.Event eventWwise;
        public float time;

        public AnimationEventData(AnimationClip _clip, AK.Wwise.Event _eventWwise, float _time)
        {
            clip = _clip;
            eventWwise = _eventWwise;
            time = _time;
        }
    }

    public List<AnimationEventData> animationEvents = new List<AnimationEventData>();
    public bool debugLogsEnabled = false; 

    void Start()
    {
        foreach (var animationEvent in animationEvents)
        {
            if (animationEvent.clip != null)
            {
                AnimationEvent evt = new AnimationEvent();
                evt.time = animationEvent.time;
                evt.functionName = "InvokeMethod";
                evt.stringParameter = animationEvent.eventWwise.Name;
                animationEvent.clip.AddEvent(evt);
            }
        }
    }

    void InvokeMethod(string eventName)
    {
        AnimationClip currentClip = GetCurrentAnimationClip();
        if (currentClip != null)
        {
            if (debugLogsEnabled)
            {
                Debug.Log("Animation Event Triggered - Animation Clip: " + currentClip.name + ", Event: " + eventName);
            }
            AkSoundEngine.PostEvent(eventName, gameObject);
        }
        else
        {
            Debug.LogWarning("No animation clip is currently playing.");
        }
    }
    AnimationClip GetCurrentAnimationClip()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                return clipInfo[0].clip;
            }
        }
        return null;
    }
}
