using UnityEngine;

namespace Vrlife.Core.Vr
{
    [CreateAssetMenu(fileName = "Xr General Settings", menuName = "XR/General Settings")]
    public class XrGeneralSettings : ScriptableObject
    {
        public float minTriggerPressureToClick = .8f;
    }
}