using UnityEngine;
using AK.Wwise;

public class RTPCControllerVolume : MonoBehaviour
{
    public AK.Wwise.RTPC rtpc1;
    [Range(-96f, 0f)]
    public float sliderRtpc01;

    private void Update()
    {
        rtpc1.SetGlobalValue(sliderRtpc01);
    }
}
