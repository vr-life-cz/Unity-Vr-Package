using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [Header("Controls")]
        [Description("Enables teleportation around using B button on right controller.")]
        public bool teleportationEnabled = true;
        [Description("Changes whether the teleportation is a straight line or a ballistic curve")]
        public bool useBallisticTeleportation = true;
        
        [Description("Enables 'Walking' along X and Z axis using right joystick.")]
        public bool horizontalMovementEnabled = true;
        [Description("Enables 'Flying' along Y axis using left joystick.")]
        public bool verticalMovementEnabled = true;
        [Description("Level of the floor. You can't go below it.")] 
        public float floorLevel = 0f;

        private RaycastHit hitObject;
        private bool teleportReady;

        private Gradient canTeleportGradient;
        private Gradient cantTeleportGradient;

        private void EnableDefaultMovement()
        {
            verticalMovementEnabled = true;
            horizontalMovementEnabled = false;
        }
        
        public void DisableBothMovement()
        {
            verticalMovementEnabled = false;
            horizontalMovementEnabled = false;
        }

        public void SwitchMovement()
        {
            if (verticalMovementEnabled || horizontalMovementEnabled)
            {
                if (verticalMovementEnabled)
                {
                    verticalMovementEnabled = false;
                    horizontalMovementEnabled = true;
                }
                else
                {
                    verticalMovementEnabled = true;
                    horizontalMovementEnabled = false;
                }
            }
        }
        
        public void ToggleVerticalMovement()
        {
            if (!verticalMovementEnabled)
            {
                verticalMovementEnabled = true;
                horizontalMovementEnabled = false;
            }
            else
            {
                verticalMovementEnabled = false;
            }
        }
        
        public void ToggleHorizontalMovement()
        {
            if (!horizontalMovementEnabled)
            {
                horizontalMovementEnabled = true;
                verticalMovementEnabled = false;
            }
            else
            {
                horizontalMovementEnabled = false;
            }
        }
        
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
            if (teleportationEnabled) Teleport();
        }

       
        
        public void HorizontalMovement(InteractionInformation interactionInformation)
        {
            if (!horizontalMovementEnabled) return;
            Vector2 input = interactionInformation.JoystickPosition;

            Vector3 forward = _camera.transform.forward;
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = _camera.transform.right;
            right.y = 0f;
            right.Normalize();

            Vector3 coordinates = right * input.x + forward * input.y;
            transform.Translate(coordinates * movementSpeed * Time.deltaTime);
        }

        public void VerticalMovement(InteractionInformation interactionInformation)
        {
            if (!verticalMovementEnabled) return;
            Vector2 input = interactionInformation.JoystickPosition;
            Vector3 coordinates = Vector3.up * input.y;
            if (_camera.transform.position.y > (floorLevel + .5f) || input.y > 0)
            {
                transform.Translate(coordinates * movementSpeed * Time.deltaTime);
            }
            
        }

        private void Teleport()
        {
            if (inputUpdater.RightHandInputDevice.InteractionInformation.IsPrimaryButtonClicked)
            {
                if (useBallisticTeleportation)
                {
                    float decayRate = 0.05f;
                    float stepDistance = .5f;
                    int maxSteps = 128;
                    int currentSteps = 0;
                    bool rayHit = false;

                    Vector3 startingPosition = pointerHand.transform.position;
                    Vector3 pointingDirection = pointerHand.transform.forward.normalized;
                    Vector3 currentPosition = startingPosition;

                    List<Vector3> positions = new List<Vector3> {startingPosition};
                    pointer.positionCount = 1;


                    while (currentSteps <= maxSteps && currentPosition.y >= floorLevel && !rayHit)
                    {
                        
                        Vector3 decayedVector = Mathf.Clamp01(decayRate * currentSteps) * Vector3.down +
                                                pointingDirection * Mathf.Clamp01(1 - decayRate * currentSteps);
                        
                        decayedVector = decayedVector.normalized * stepDistance; 


                        Ray raycast = new Ray(currentPosition, decayedVector);
                        Physics.Raycast(raycast, out hitObject, stepDistance, layerMask);

                        rayHit = !(hitObject.transform is null);

                        currentPosition += decayedVector;
                        
                        currentSteps++;
                        
                        positions.Add(currentPosition);

                        pointer.positionCount++;
                        pointer.SetPositions(positions.ToArray());
                    }
                    pointer.enabled = true;

                    if (rayHit)
                    {
                        pointer.startColor = canTeleportColor;
                        pointer.endColor = canTeleportColor;
                        teleportReady = true;
                    }
                    else
                    {
                        pointer.startColor = cantTeleportColor;
                        pointer.endColor = cantTeleportColor;
                    }
                    

                    
                }
                else
                {
                    Ray raycast = new Ray(_camera.transform.position, pointerHand.transform.forward.normalized);
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
                            pointerHand.transform.position + pointerHand.transform.forward
                        });
                        pointer.enabled = true;
                    }
                }
                
            }
            else
            {
                if (teleportReady)
                {
                    transform.position = new Vector3(hitObject.point.x - _camera.transform.localPosition.x, hitObject.point.y, hitObject.point.z - _camera.transform.localPosition.z);
                    teleportReady = false;
                }
                pointer.enabled = false;
            }
        }
    }
}