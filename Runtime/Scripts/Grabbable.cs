using UnityEngine;

namespace Vrlife.Core.Vr
{
    [RequireComponent(typeof(Collider))]
    public class Grabbable : MonoBehaviour
    {
        public bool worldPositionStays = true;

        public bool returnToOriginalParentOnRelease = true;

        public Grabber grabedBy;

        public GrabableEventHandle onGrabbed;


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
            onGrabbed?.Invoke(this);
        }

        public void InvokeOnReleased()
        {
            if (returnToOriginalParentOnRelease)
            {
                if (!_parent)
                {
                    Debug.LogError("No parent");
                }

                transform.SetParent(_parent);

                transform.localPosition = _localOffset;
                transform.rotation = _original;
            }

            onReleased?.Invoke(this);
        }
    }
}