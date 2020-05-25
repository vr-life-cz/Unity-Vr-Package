using System.Linq;
using UnityEngine;

namespace Vrlife.Core.Vr
{
    public class PlayerHandController : IPlayerHandController
    {
        private IPlayerHandView _view;

        private readonly XrGeneralSettings _generalSettings;

        private readonly IPlayerInputUpdater _inputUpdater;

        private bool _triggerClicked;

        private bool _onReleaseFired;

        private bool _onGrabFired;


        public PlayerHandController(IPlayerInputUpdater inputUpdater, XrGeneralSettings generalSettings)
        {
            _inputUpdater = inputUpdater;
            _generalSettings = generalSettings;
        }

        public void BindView(IPlayerHandView view)
        {
            if (_view != null && _view.Watcher)
            {
                _view.Watcher.onProximityTriggerEnter.RemoveListener(OnEnter);
                _view.Watcher.onProximityTriggerEnter.RemoveListener(OnEnter);
            }

            _view = view;

            if (_view != null && _view.Watcher)
            {
                _view.Watcher.onProximityTriggerEnter.AddListener(OnEnter);
                _view.Watcher.onProximityTriggerEnter.AddListener(OnEnter);
            }
        }

        private readonly int AnimatorFinger = Animator.StringToHash("Blend");

        private void FireInputHandler(ControllerInput input)
        {
            var triggerClicks = _view.InputBindings.Where(x => x.input == input);

            foreach (var inputBinding in triggerClicks)
            {
                inputBinding.handler?.Invoke();
            }
        }

        private bool _isPrimaryButtonDown;
        private bool _isSecondaryButtonDown;

        public void Update()
        {
            var handRootTransform = _view.HandRootTransform;

            var inputDevice = _view.HandType == HumanBodyPart.LeftHand
                ? _inputUpdater.LeftHandInputDevice
                : _inputUpdater.RightHandInputDevice;


            var trackingInformation = inputDevice.TrackingInformation;
            ;

            handRootTransform.localPosition = trackingInformation.Position;

            handRootTransform.localRotation = trackingInformation.Rotation;

            _view.Animator.SetParameter(AnimatorFinger, inputDevice.InteractionInformation.TriggerPressure);

            if (!_onGrabFired && _generalSettings.minTriggerPressureToClick <
                inputDevice.InteractionInformation.TriggerPressure)
            {
                _onGrabFired = true;
                FireInputHandler(ControllerInput.TriggerClick);
            }
            else if (_onGrabFired && _generalSettings.minTriggerPressureToClickRelease >
                     inputDevice.InteractionInformation.TriggerPressure)
            {
                _onGrabFired = false;
            }


            if (!_isPrimaryButtonDown && inputDevice.InteractionInformation.IsPrimaryButtonClicked)
            {
                _isPrimaryButtonDown = true;
                FireInputHandler(ControllerInput.PrimaryButtonClick);
            }
            else if (_isPrimaryButtonDown && !inputDevice.InteractionInformation.IsPrimaryButtonClicked))
            {
                _isPrimaryButtonDown = false;
            }


            if (!_isSecondaryButtonDown && inputDevice.InteractionInformation.IsSecondaryButtonClicked)
            {
                _isSecondaryButtonDown = true;
                FireInputHandler(ControllerInput.SecondaryButtonClick);
            }
            else if (_isSecondaryButtonDown && !inputDevice.InteractionInformation.IsSecondaryButtonClicked)
            {
                _isSecondaryButtonDown = false;
            }

//           
//            if (_generalSettings.minTriggerPressureToClick < inputDevice.InteractionInformation.TriggerPressure)
//            {
//                if (!_onGrabFired)
//                {
//                    Debug.Log("clicked");
//
//                    _onGrabFired = true;
//                    _onReleaseFired = false;
//                    _view.OnTriggerClicked?.Invoke(inputDevice);
//                }
//            }
//            else
//            {
//                if (!_onReleaseFired)
//                {
//                    Debug.Log("released");
//                    
//                    _onGrabFired = false;
//                    _onReleaseFired = true;
//                    _view.OnTriggerReleased?.Invoke(inputDevice);
//                }
//            }
        }

        private void OnEnter(ProximityWatcher sender, Collider args)
        {
            _inputUpdater.SendHapticFeedback(_view.HandType);
        }
    }
}