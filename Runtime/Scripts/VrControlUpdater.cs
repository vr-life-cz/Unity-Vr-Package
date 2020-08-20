using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.XR;
using Zenject;

namespace Vrlife.Core.Vr
{
    public struct HapticSignal
    {
        public static HapticSignal Default => new HapticSignal
        {
            Duration = 1f,
            Amplitude = 0.5f
        };

        public float Amplitude { get; set; }

        public float Duration { get; set; }
    }

    public class VrControlUpdater : MonoBehaviour, IPlayerInputUpdater
    {
        private List<XRInputSubsystem> _subsystems;

        private List<XRNodeState> _nodes;
        public PlayerHandInputDevice LeftHandInputDevice { get; private set; }
        public PlayerHandInputDevice RightHandInputDevice { get; private set; }

        [Inject] private XrGeneralSettings settings;

        public void SendHapticFeedback(HumanBodyPart handType)
        {
            if (!settings.allowVibration) return;
            switch (handType)
            {
                case HumanBodyPart.LeftHand:
                    if (_leftHandImpulses.Count >= 1 && !settings.allowVibrationStacking) return;
                    _leftHandImpulses.Push(HapticSignal.Default);
                    break;
                case HumanBodyPart.RightHand:
                    if (_rightHandImpulses.Count >= 1 && !settings.allowVibrationStacking) return;
                    _rightHandImpulses.Push(HapticSignal.Default);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(handType), handType, null);
            }
        }

        private Stack<HapticSignal> _leftHandImpulses = new Stack<HapticSignal>();
        private Stack<HapticSignal> _rightHandImpulses = new Stack<HapticSignal>();

        private byte[] impulse;

        private void Awake()
        {
            _nodes = new List<XRNodeState>();

            _subsystems = new List<XRInputSubsystem>();
        }

        private void Start()
        {
            LeftHandInputDevice = new PlayerHandInputDevice(HumanBodyPart.LeftHand, settings);

            RightHandInputDevice = new PlayerHandInputDevice(HumanBodyPart.RightHand, settings);

            SubsystemManager.GetInstances(_subsystems);

            foreach (var inputSubsystem in _subsystems)
            {
                Debug.Log("Detected subsystem " + inputSubsystem.GetType());
                inputSubsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor);
            }


            if (_subsystems.Count == 0)
            {
                XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
                Debug.LogWarning("No Subsystems detected");
            }

            impulse = new byte[20];

            var caps = new HapticCapabilities();

            int clipCount = (int) (caps.bufferFrequencyHz * 2);

            impulse = new byte[clipCount];

            for (int i = 0; i < clipCount; i++)
            {
                impulse[i] = byte.MaxValue;
            }
        }

        private bool _errorLogged;

        private void Update()
        {
            _nodes.Clear();


            var inputDevices = new List<InputDevice>();

            InputDevices.GetDevices(inputDevices);

            if (inputDevices.Count == 0 && !_errorLogged)
            {
                _errorLogged = true;
                Debug.LogError("No Input devices");
            }

            foreach (var inputDevice in inputDevices)
            {
                inputDevice.SendHapticBuffer(0, impulse);

                if ((inputDevice.characteristics & InputDeviceCharacteristics.Left) != 0)
                {
                    if (_leftHandImpulses.Count > 0)
                    {
                        var signalL = _leftHandImpulses.Pop();
                        inputDevice.SendHapticImpulse(0, signalL.Amplitude, signalL.Duration);
                    }

                    UpdateTrackingInformation(inputDevice, LeftHandInputDevice);
                }


                if ((inputDevice.characteristics & InputDeviceCharacteristics.Right) != 0)
                {
                    if (_rightHandImpulses.Count > 0)
                    {
                        var signalR = _rightHandImpulses.Pop();
                        inputDevice.SendHapticImpulse(0, signalR.Amplitude, signalR.Duration);
                    }

                    UpdateTrackingInformation(inputDevice, RightHandInputDevice);
                }
            }
        }

        private void UpdateTrackingInformation(InputDevice inputDevice, PlayerHandInputDevice info)
        {
            inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out info.TrackingInformation.Position);

            inputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out info.TrackingInformation.Rotation);

            inputDevice.TryGetFeatureValue(CommonUsages.deviceAcceleration, out info.TrackingInformation.Acceleration);

            inputDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out info.TrackingInformation.Velocity);

            inputDevice.TryGetFeatureValue(CommonUsages.triggerButton,
                out info.InteractionInformation.IsTriggerTouched);

            inputDevice.TryGetFeatureValue(CommonUsages.trigger, out info.InteractionInformation.TriggerPressure);

            inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out info.InteractionInformation.IsGripTouched);

            inputDevice.TryGetFeatureValue(CommonUsages.grip, out info.InteractionInformation.GripPressure);

            inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis,
                out info.InteractionInformation.JoystickPosition);

            inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisTouch,
                out info.InteractionInformation.IsJoystickTouched);

            inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick,
                out info.InteractionInformation.IsJoystickClicked);

            inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, // A/X
                out info.InteractionInformation.IsPrimaryButtonClicked);

            inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, // B/Y
                out info.InteractionInformation.IsSecondaryButtonClicked);
        }
    }
}