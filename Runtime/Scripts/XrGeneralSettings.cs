using UnityEngine;

namespace Vrlife.Core.Vr
{
    [CreateAssetMenu(fileName = "Xr General Settings", menuName = "XR/General Settings")]
    public class XrGeneralSettings : ScriptableObject
    {
        public float minTriggerPressureToClick = .8f;
        public float minTriggerPressureToClickRelease = .2f;

        public bool allowVibration = true;
        [Tooltip("Allows many instances of haptic feedback to be stacked one after another.")]
        public bool allowVibrationStacking = false;
    }
}