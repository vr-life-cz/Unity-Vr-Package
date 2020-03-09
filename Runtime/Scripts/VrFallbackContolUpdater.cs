using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

namespace Vrlife.Core.Vr
{
    public class VrFallbackContolUpdater : MonoBehaviour, IPlayerInputUpdater
    {
        public PlayerHandInputDevice LeftHandInputDevice { get; private set; }
        public PlayerHandInputDevice RightHandInputDevice { get; private set; }
        public void SendHapticFeedback(HumanBodyPart handType)
        {
            
        }

        private HumanBodyPart _controlledHand = HumanBodyPart.RightHand;

        private float distance = 0.8f;

        private void Start()
        {
            LeftHandInputDevice = new PlayerHandInputDevice(HumanBodyPart.LeftHand);
            RightHandInputDevice = new PlayerHandInputDevice(HumanBodyPart.RightHand);
        }

        public KeyCode ControlCamera = KeyCode.LeftShift;

        public KeyCode CameraForward = KeyCode.W;
        public KeyCode cameraBack = KeyCode.S;
        public KeyCode cameraRight = KeyCode.D;
        public KeyCode cameraLeft = KeyCode.A;
        public KeyCode cameraUp = KeyCode.Q;
        public KeyCode cameraDown = KeyCode.E;
        
        
        public float cameraSpeed = 0.01f;

        private Vector3 _lastMousePos;
        public float speedH = 2.0f;
        public float speedV = 2.0f;

        private float yaw = 0.0f;
        private float pitch = 0.0f;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _controlledHand = _controlledHand == HumanBodyPart.LeftHand
                    ? HumanBodyPart.RightHand
                    : HumanBodyPart.LeftHand;
            }

            if (Input.GetKey(ControlCamera))
            {
                var transform1 = Camera.main.transform;

                if (Input.GetKey(CameraForward))
                {
                    transform1.position += transform1.forward * cameraSpeed;
                }

                if (Input.GetKey(cameraBack))
                {
                    transform1.position += -1 * cameraSpeed * transform1.forward;
                }

                if (Input.GetKey(cameraLeft))
                {
                    transform1.position += -1 * cameraSpeed * transform1.right;
                }

                if (Input.GetKey(cameraRight))
                {
                    transform1.position += transform1.right * cameraSpeed;
                }
 if (Input.GetKey(cameraUp))
                {
                    transform1.position += transform1.up * cameraSpeed;
                }
 if (Input.GetKey(cameraDown))
                {
                    transform1.position += transform1.up * -cameraSpeed;
                }

                if (Input.GetMouseButton((int) MouseButton.RightMouse))
                {
                    yaw += speedH * Input.GetAxis("Mouse X");
                    pitch -= speedV * Input.GetAxis("Mouse Y");

                    transform1.eulerAngles = new Vector3(pitch, yaw, 0.0f);
                }

                return;
            }
            else
            {
                UpdateTrackingInformations(_controlledHand == HumanBodyPart.RightHand ? RightHandInputDevice : LeftHandInputDevice);
            }

            _lastMousePos = Input.mousePosition;
        }

        private void UpdateTrackingInformations(PlayerHandInputDevice info)
        {
            distance += Input.mouseScrollDelta.y * .1f;

            var v3 = Input.mousePosition;
            
            
            info.TrackingInformation.Position = Camera.main.ScreenToWorldPoint( Input.mousePosition);
            
            info.InteractionInformation.IsTriggerTouched = Input.GetAxis("Fire1") > 0;
            
            info.InteractionInformation.TriggerPressure = Input.GetAxis("Fire1");
        }
    }
}