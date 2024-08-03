using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WwisePlayerAnimationEvents : MonoBehaviour
{
    public AK.Wwise.Event playerIdle;
    public AK.Wwise.Event playerSpawn;
    public AK.Wwise.Event playerDeath;
    public AK.Wwise.Event playerHurt;
    public AK.Wwise.Event playerVictory;
    public AK.Wwise.Event whoosh;
    public void AnimPlayerIdle() { AkSoundEngine.PostEvent(playerIdle.Id, gameObject); }
    public void AnimPlayerSpawn() { AkSoundEngine.PostEvent(playerSpawn.Id, gameObject); }
    public void AnimPlayerDeath() { AkSoundEngine.PostEvent(playerDeath.Id, gameObject); }
    public void AnimPlayerHurt() { AkSoundEngine.PostEvent(playerHurt.Id, gameObject); }
    public void AnimPlayerVictory() { AkSoundEngine.PostEvent(playerVictory.Id, gameObject); }
    public void Animwhoosh() { AkSoundEngine.PostEvent(whoosh.Id, gameObject); }

}

