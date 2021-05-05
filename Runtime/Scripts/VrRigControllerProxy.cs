using System;
using System.Collections.Generic;
using System.ComponentModel;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Vrlife.Core.Vr
{
    public class VrRigControllerProxy : MonoBehaviour
    {
        [Inject] public IPlayerInputUpdater inputUpdater;

        [BoxGroup("Dependencies")] public Camera _camera;

        [BoxGroup("Dependencies")] public GameObject pointerHand;
        [BoxGroup("Dependencies")] public LineRenderer pointer;
        [BoxGroup("Dependencies")] public Animator fadeOutAnimator;

        [BoxGroup("Settings")] [Range(0f, 2f)] public float movementSpeed = 1f;

        [BoxGroup("Settings")] public LayerMask layerMask;

        [BoxGroup("Settings")] [ColorUsage(true, false)]
        public Color canTeleportColor;

        [BoxGroup("Settings")] [ColorUsage(true, false)]
        public Color cantTeleportColor;

        [BoxGroup("Controls")] [Description("Enables teleportation around using B button on right controller.")]
        public bool teleportationEnabled = true;

        [BoxGroup("Controls")]
        [Description("Changes whether the teleportation is a straight line or a ballistic curve")]
        public bool useBallisticTeleportation = true;

        [BoxGroup("Controls")] [Description("Enables 'Walking' along X and Z axis using right joystick.")]
        public bool horizontalMovementEnabled = true;

        [BoxGroup("Controls")] [Description("Enables 'Flying' along Y axis using left joystick.")]
        public bool verticalMovementEnabled = true;

        [BoxGroup("Controls")] [Description("Level of the floor. You can't go below it.")]
        public float floorLevel = 0f;

        [BoxGroup("Controls")] [Description("If enabled, the horizontal and vertical movement will respect colliders")]
        public bool respectColliders;

        private RaycastHit hitObject;
        private bool teleportReady;

        private Gradient canTeleportGradient;
        private Gradient cantTeleportGradient;
        private static readonly int Fadein = Animator.StringToHash("fadein");
        private static readonly int Fadeout = Animator.StringToHash("fadeout");

        private void Awake()
        {
            layerMask = LayerMask.NameToLayer("Everything");

            canTeleportGradient = new Gradient();
            canTeleportGradient.SetKeys(
                new GradientColorKey[]
                    {new GradientColorKey(canTeleportColor, 0.0f), new GradientColorKey(Color.white, 1.0f)},
                new GradientAlphaKey[]
                    {new GradientAlphaKey(1f, 0.0f), new GradientAlphaKey(1f, 0.95f), new GradientAlphaKey(0f, 1f)}
            );

            cantTeleportGradient = new Gradient();
            cantTeleportGradient.SetKeys(
                new GradientColorKey[]
                    {new GradientColorKey(cantTeleportColor, 0.0f), new GradientColorKey(Color.white, 1.0f)},
                new GradientAlphaKey[]
                    {new GradientAlphaKey(1f, 0.0f), new GradientAlphaKey(1f, 0.95f), new GradientAlphaKey(0f, 1f)}
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

            // Respecting colliders

            if (respectColliders)
            {
                Vector3 cameraPosition = _camera.transform.position;

                // walls
                Ray raycast = new Ray(cameraPosition, coordinates);
                if (Physics.SphereCast(raycast, .1f, .5f))
                {
                    return;
                }

                // slanted surfaces / stairs
                Vector3 movementPoint = new Vector3(coordinates.x, transform.position.y, coordinates.z);
                Vector3 onTheGround = new Vector3(movementPoint.x / 2f, movementPoint.y - cameraPosition.y,
                    movementPoint.z / 2f);

                float cameraHeight = _camera.transform.localPosition.y;

                Ray surfaceRay = new Ray(cameraPosition, onTheGround);

                Physics.SphereCast(surfaceRay, .1f, out RaycastHit hit);

                float difference = cameraHeight - hit.distance;

                if (Math.Abs(difference) >= 0.1f && hit.point != Vector3.zero)
                {
                    coordinates = new Vector3(coordinates.x, coordinates.y + difference, coordinates.z);
                }
            }

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
                    float stepDistance = 1f;
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
                    transform.position = new Vector3(hitObject.point.x - _camera.transform.localPosition.x,
                        hitObject.point.y, hitObject.point.z - _camera.transform.localPosition.z);
                    teleportReady = false;
                }

                pointer.enabled = false;
            }
        }

        [Button]
        public void DarkenScreen()
        {
            fadeOutAnimator.SetTrigger(Fadein);
        }

        [Button]
        public void UnDarkScreen()
        {
            fadeOutAnimator.SetTrigger(Fadeout);
        }
    }
}