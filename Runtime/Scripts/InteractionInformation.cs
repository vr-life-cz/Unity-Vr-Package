using System;
using UnityEngine;

namespace Vrlife.Core.Vr
{
    [Serializable]

    public class InteractionInformation
    {
        private HumanBodyPart Part { get; }

        public bool IsJoystickClicked;

        public bool IsJoystickTouched;

        public Vector2 JoystickPosition;

        public float GripPressure;

        public bool IsGripTouched;

        public float TriggerPressure;

        public bool IsTriggerTouched;

        public InteractionInformation(HumanBodyPart part)
        {
            Part = part;
        }
    }
}