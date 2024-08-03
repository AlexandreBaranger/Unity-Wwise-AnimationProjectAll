using UnityEngine;

public class WwiseAnimationSelectorAttribute : PropertyAttribute
{
    public System.Type[] TargetTypes;

    public WwiseAnimationSelectorAttribute(params System.Type[] targetTypes)
    {
        this.TargetTypes = targetTypes;
    }
}
