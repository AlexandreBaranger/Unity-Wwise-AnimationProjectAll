using UnityEngine;

public class RTPCControllerSynth : MonoBehaviour
{
    public AK.Wwise.RTPC rtpc1;
    [Range(-1f, 1f)]
    public float sliderRtpc01;

    private void Update()
    {
        rtpc1.SetGlobalValue(sliderRtpc01);
    }
}
