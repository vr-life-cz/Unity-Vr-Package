using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Vrlife.Core.Vr
{
    public class VrControllerProxy : MonoBehaviour
    {
        [SerializeField] private HumanBodyPart part;
        
        private IPlayerInputUpdater _playerInputUpdater;

        [SerializeField] private Dictionary<ControllerInput, List<UnityEvent>> inputHandlers;

        private XrGeneralSettings _settings;

        private PlayerHandInputDevice _device;
        
        [Inject]
        private void Ctor(IPlayerInputUpdater inputUpdater, XrGeneralSettings settings )
        {
            _device = part == HumanBodyPart.LeftHand ? inputUpdater.LeftHandInputDevice :
                part == HumanBodyPart.RightHand ? inputUpdater.RightHandInputDevice :
                throw new ArgumentOutOfRangeException("Please select valid value");
            
            _settings = settings;
            
            _playerInputUpdater = inputUpdater;
        }

        private bool _isTriggerClicked;
        
        void Update()
        {
            if (!_isTriggerClicked &&
                _device.InteractionInformation.TriggerPressure > _settings.minTriggerPressureToClick)
            {
                _isTriggerClicked = true;
                 
                if(inputHandlers.TryGetValue(ControllerInput.TriggerClick, out var handlers))
                {
                    foreach (var unityEvent in handlers)
                    {
                        unityEvent?.Invoke();
                    }
                }
            }
            else if(_isTriggerClicked && _device.InteractionInformation.TriggerPressure < _settings.minTriggerPressureToClickRelease)
            {
                _isTriggerClicked = false;
               
            }
        }
    }
}