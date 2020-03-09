using System.Text;
using UnityEngine;
using UnityEngine.XR;
using Zenject;

namespace Vrlife.Core.Vr
{
    
    public interface IPlayerHandInteractionService
    {
        void CanGrabItem(PlayerHandInputDevice leftHandInputDevice);
    }

    public class PlayerHandInteractionService : IPlayerHandInteractionService
    {
        public void CanGrabItem(PlayerHandInputDevice leftHandInputDevice)
        {
            
        }
    }


    [RequireComponent(typeof(Rigidbody))]
    public class PlayerHandViewModelDelete : MonoBehaviour, IDebugInfoProvider
    {
        [Inject] public IPlayerInputUpdater inputUpdater;

        public HumanBodyPart handType = HumanBodyPart.LeftHand;

        private StringBuilder _stringBuilder = new StringBuilder();

        public string Label => gameObject.name;


        public string GetDebugInfo()
        {
            _stringBuilder.Clear();

            _stringBuilder.AppendLine($"Tracking space - {XRDevice.GetTrackingSpaceType()}");

            var LeftHand = handType == HumanBodyPart.LeftHand
                ? inputUpdater.LeftHandInputDevice.TrackingInformation
                : inputUpdater.RightHandInputDevice.TrackingInformation;

            if (LeftHand != null)
            {
                _stringBuilder.AppendLine($"Left Hand");
                _stringBuilder.AppendLine($"Position - {LeftHand.Position}");
                _stringBuilder.AppendLine($"Rotation - {LeftHand.Rotation}");
                _stringBuilder.AppendLine($"Acceleration - {LeftHand.Acceleration}");
                _stringBuilder.AppendLine($"Velocity - {LeftHand.Velocity}");
            }

            return _stringBuilder.ToString();
        }

        private void Update()
        {
            var transformRef = transform;
            var _trackingInformation = handType == HumanBodyPart.LeftHand
                ? inputUpdater.LeftHandInputDevice.TrackingInformation
                : inputUpdater.RightHandInputDevice.TrackingInformation;
            ;
            transformRef.localPosition = _trackingInformation.Position;

            transformRef.localRotation = _trackingInformation.Rotation;
        }
    }
}