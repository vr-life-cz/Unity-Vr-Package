using System.Linq;
using UnityEngine;

namespace Vrlife.Core.Vr
{
    public class GrabService : IGrabService
    {
        private readonly IPlayerInputUpdater _inputUpdater;

        public GrabService(IPlayerInputUpdater inputUpdater)
        {
            _inputUpdater = inputUpdater;
        }

        public void Grab(Grabber possessor)
        {
            if (!possessor || !possessor.proximityWatcher || possessor.proximityWatcher.ProximityObjects.Count == 0)
            {
                return;
            }

            var grabbable = possessor.proximityWatcher.ProximityObjects.Select(x => x.GetComponent<Grabbable>())
                .FirstOrDefault(x => x != null);

            if (grabbable == null) return;

            if (possessor.grabbedObject)
            {
                Release(possessor);
            }

            possessor.grabbedObject = grabbable;

            grabbable.grabedBy = possessor;

            grabbable.transform.SetParent(possessor.transform, grabbable.worldPositionStays);

            grabbable.InvokeOnGrabbed();
        }

        public void Release(Grabber possessor)
        {
            if (!possessor.grabbedObject) return;

            possessor.grabbedObject.grabedBy = null;
            possessor.grabbedObject.transform.SetParent(null);
           
            if (possessor.part != HumanBodyPart.Unknown)
            {
                var device = possessor.part == HumanBodyPart.LeftHand
                    ? _inputUpdater.LeftHandInputDevice
                    : _inputUpdater.RightHandInputDevice;

                var rigid = possessor.grabbedObject.GetComponent<Rigidbody>();
                if (rigid)
                {
                    rigid.velocity = device.TrackingInformation.Velocity;
                    Debug.Log(rigid.velocity);
                }
            }
            
            possessor.grabbedObject.InvokeOnReleased();
            possessor.grabbedObject = null;
            
            
           

        }
    }
}