using System;
using UnityEngine;
using Zenject;

namespace Vrlife.Core.Vr
{
    public class VrRigControllerProxy : MonoBehaviour
    {
        [Inject] public IPlayerInputUpdater inputUpdater;

        public Camera _camera;
        [Range(0f, 2f)] public float movementSpeed = 1f;
        public GameObject pointerHand;
        public LineRenderer pointer;
        public LayerMask layerMask;
        [ColorUsage(true, false)]
        public Color canTeleportColor;
        [ColorUsage(true, false)]
        public Color cantTeleportColor;

        private RaycastHit hitObject;
        private bool teleportReady;

        private Gradient canTeleportGradient;
        private Gradient cantTeleportGradient;


        private void Awake()
        {
            layerMask = LayerMask.NameToLayer("Everything");

            canTeleportGradient = new Gradient();
            canTeleportGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(canTeleportColor, 0.0f), new GradientColorKey(Color.white, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0.0f), new GradientAlphaKey(1f, 0.95f), new GradientAlphaKey(0f, 1f) }
            );    
        
            cantTeleportGradient = new Gradient();
            cantTeleportGradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(cantTeleportColor, 0.0f), new GradientColorKey(Color.white, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0.0f), new GradientAlphaKey(1f, 0.95f), new GradientAlphaKey(0f, 1f) }
            );
            
        }

        private void Update()
        {
            Movement();
            Teleport();
        }

        private void Movement()
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

        private void Teleport()
        {
            if (inputUpdater.RightHandInputDevice.InteractionInformation.IsPrimaryButtonClicked)
            {
                Ray raycast = new Ray(_camera.transform.position, -pointerHand.transform.forward.normalized);
                bool rayHit = Physics.Raycast(raycast, out hitObject, float.MaxValue, layerMask);
                if (rayHit)
                {
                    pointer.colorGradient = canTeleportGradient;
                    pointer.SetPositions(new Vector3[]
                    {
                        pointerHand.transform.position,
                        hitObject.point
                    });
                    pointer.enabled = true;
                    teleportReady = true;
                }
                else
                {
                    pointer.colorGradient = cantTeleportGradient;
                    pointer.SetPositions(new Vector3[]
                    {
                        pointerHand.transform.position,
                        pointerHand.transform.position - pointerHand.transform.forward
                    });
                    pointer.enabled = true;
                }
            }
            else
            {
                if (teleportReady)
                {
                    transform.position = hitObject.point;
                    teleportReady = false;
                }
                pointer.enabled = false;
            }
        }
    }
}