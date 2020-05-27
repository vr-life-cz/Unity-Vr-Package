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

        public bool IsPrimaryButtonClicked;

        public bool IsSecondaryButtonClicked;

        private bool _isTriggerClicked;

        public bool IsTriggerClicked
        {
            get
            {
                if (_isTriggerClicked)
                {
                    if (TriggerPressure < _settings.minTriggerPressureToClickRelease)
                    {
                        _isTriggerClicked = false;
                    }
                }
                else
                {
                    if (TriggerPressure > _settings.minTriggerPressureToClick)
                    {
                        _isTriggerClicked = true;
                    }
                }

                return _isTriggerClicked;
            }
            
        } 
        
        private XrGeneralSettings _settings;
        
        public InteractionInformation(HumanBodyPart part, XrGeneralSettings settings)
        {
            _settings = settings;
            Part = part;
        }
    }
}