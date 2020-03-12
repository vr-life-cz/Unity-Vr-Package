using UnityEngine;
using Zenject;

namespace Vrlife.Core.Vr
{
    public class VrRigControllerProxy : MonoBehaviour
    {
        [Inject] public IPlayerInputUpdater inputUpdater;

        public Camera _camera;
        [Range(0f, 2f)]
        public float movementSpeed = 1f;
        
        private void Update()
        {
            Vector2 input = inputUpdater.RightHandInputDevice.InteractionInformation.JoystickPosition;

            Vector3 forward = _camera.transform.forward;
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = _camera.transform.right;
            right.y = 0f;
            right.Normalize();

            Vector3 coordinates = right * input.x + forward * input.y;
            transform.Translate(coordinates * movementSpeed * Time.deltaTime);
        }
    }
}