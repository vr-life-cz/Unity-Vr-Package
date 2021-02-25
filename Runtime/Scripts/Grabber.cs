using System.Linq;
using UnityEngine;
using Zenject;

namespace Vrlife.Core.Vr
{
    public class Grabber : MonoBehaviour, IDebugInfoProvider
    {
        public ProximityWatcher proximityWatcher;

        public bool triggerControlled = true;
        public Grabbable grabbedObject;

        public HumanBodyPart part;

        [Inject] private IGrabService _grabService;

        [Inject] private IPlayerInputUpdater _playerInputUpdater;

        private bool _isGrabbed;

        private Vector3 _lastPosition;

        public Vector3 Velocity { get; private set; }

        private void Update()
        {
            var position = transform.position;

            Velocity = (position - _lastPosition);

            _lastPosition = position;

            var interaction = part == HumanBodyPart.LeftHand
                ? _playerInputUpdater.LeftHandInputDevice.InteractionInformation
                : _playerInputUpdater.RightHandInputDevice.InteractionInformation;


            if (triggerControlled)
            {
                if (interaction.TriggerPressure > 0.8f)
                {
                    if (!_isGrabbed)
                    {
                        Grab();
                    }
                }
                else if (interaction.TriggerPressure < .1f)
                {
                    if (_isGrabbed)
                    {
                        Release();
                    }
                }
            }
        }

        public void Grab()
        {
            _isGrabbed = true;
            _grabService.Grab(this);
        }

        public void Release()
        {
            _isGrabbed = false;
            _grabService.Release(this);
        }

        public string GetDebugInfo()
        {
            try
            {
                return
                    $"Proximity Obj - {proximityWatcher.ProximityObjects.FirstOrDefault()}\nGrabbed - {grabbedObject}";
            }
            catch
            {
                return "null";
            }
            
        }

        public string Label => gameObject.name;
    }
}