﻿using UnityEngine;

namespace Vrlife.Core.Vr
{
    [RequireComponent(typeof(Collider))]
    public class exGrabbable : MonoBehaviour
    {
        public bool canBeGrabbed = true;
        
        public bool worldPositionStays = true;

        public bool returnToOriginalParentOnRelease = true;

        public Grabber grabbedBy;

        public GrabableEventHandle onGrabbed;

        public Rigidbody grabbableRigidBody;

        public GrabableEventHandle onReleased;

        private Vector3 _localOffset;

        private Quaternion _original;

        private Transform _parent;

        private Collider _collider;

        private void Start()
        {
            _parent = transform.parent;

            _localOffset = transform.localPosition;

            _original = transform.rotation;

            _collider = GetComponent<Collider>();

        }

        private void OnEnable()
        {
            if (_collider)
            {
                _collider.enabled = true;
            }
        }

        private void OnDisable()
        {
            if (_collider)
            {
                _collider.enabled = false;
            }
        }

        public void InvokeOnGrabbed()
        {
            if (canBeGrabbed) onGrabbed?.Invoke(this);
        }

        public void InvokeOnReleased(Vector3 velocity)
        {
            if (returnToOriginalParentOnRelease)
            {
                if (_parent)
                {
                    transform.SetParent(_parent);
                }

                if (grabbableRigidBody)
                {
                    grabbableRigidBody.velocity = velocity;
                }

                transform.localPosition = _localOffset;
                transform.rotation = _original;
            }

            onReleased?.Invoke(this);
        }
    }
}