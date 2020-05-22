﻿using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Vrlife.Core.Vr
{
    public class VrInputModule : BaseInputModule
    {
        private Camera UICamera;

        private PhysicsRaycaster raycaster;

        [Inject] private IPlayerInputUpdater _playerInputUpdater;
        public LayerMask layerMask;

        private void Start()
        {
            _controllerDatas = new[]
            {
                new ControllerData
                {
                    Device = _playerInputUpdater.RightHandInputDevice
                },
                new ControllerData
                {
                    Device = _playerInputUpdater.LeftHandInputDevice
                },
            
            };

            // Create a new camera that will be used for raycasts
            UICamera = new GameObject("UI Camera").AddComponent<Camera>();

            UICamera.transform.SetParent(transform, false);

            // Added PhysicsRaycaster so that pointer events are sent to 3d objects
            raycaster = UICamera.gameObject.AddComponent<PhysicsRaycaster>();
            UICamera.clearFlags = CameraClearFlags.Nothing;
            UICamera.enabled = false;
            UICamera.fieldOfView = 5;
            UICamera.nearClipPlane = 0.01f;

            // Find canvases in the scene and assign our custom
            // UICamera to them
            Canvas[] canvases = Resources.FindObjectsOfTypeAll<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                canvas.worldCamera = UICamera;
            }
        }

// storage class for controller specific data
        private class ControllerData
        {
            public PlayerHandInputDevice Device;
            public LaserPointerEventData pointerEvent;
            public GameObject currentPoint;
            public GameObject currentPressed;
            public GameObject currentDragging;
        };

        private ControllerData[] _controllerDatas;

        protected void UpdateCameraPosition(PlayerHandInputDevice controller)
        {
            transform.position = controller.TrackingInformation.Position;
            transform.rotation = controller.TrackingInformation.Rotation;
        }
        // clear the current selection
        public void ClearSelection()
        {
            if(base.eventSystem.currentSelectedGameObject) {
                base.eventSystem.SetSelectedGameObject(null);
            }
        }

        // select a game object
        private void Select(GameObject go)
        {
            ClearSelection();

            if(ExecuteEvents.GetEventHandler<ISelectHandler>(go)) {
                base.eventSystem.SetSelectedGameObject(go);
            }
        }
    
        public override void Process()
        {
        
            raycaster.eventMask = layerMask;
            foreach (var inputDevice in _controllerDatas)
            {
                ControllerData data = inputDevice;
                // Test if UICamera is looking at a GUI element
                UpdateCameraPosition(data.Device);

                if (data.pointerEvent == null)
                    data.pointerEvent = new LaserPointerEventData(eventSystem);
                else
                    data.pointerEvent.Reset();

                data.pointerEvent.delta = Vector2.zero;
            
                data.pointerEvent.position = new Vector2(UICamera.pixelWidth * 0.5f, UICamera.pixelHeight * 0.5f);
            
                //data.pointerEvent.scrollDelta = Vector2.zero;

                Debug.DrawRay(transform.position, transform.forward, Color.red);
            
                // trigger a raycast
                eventSystem.RaycastAll(data.pointerEvent, m_RaycastResultCache);
            
                data.pointerEvent.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            
                m_RaycastResultCache.Clear();

                // make sure our controller knows about the raycast result
                // we add 0.01 because that is the near plane distance of our camera and we want the correct distance
//            if (data.pointerEvent.pointerCurrentRaycast.distance > 0.0f)
//                controller.LimitLaserDistance(data.pointerEvent.pointerCurrentRaycast.distance + 0.01f);

                // stop if no UI element was hit
                //if(pointerEvent.pointerCurrentRaycast.gameObject == null)
                //return;

                // Send control enter and exit events to our controller
                var hitControl = data.pointerEvent.pointerCurrentRaycast.gameObject;
//            if (data.currentPoint != hitControl)
//            {
//                if (data.currentPoint != null)
//                    controller.OnExitControl(data.currentPoint);
//
//                if (hitControl != null)
//                    controller.OnEnterControl(hitControl);
//            }

                data.currentPoint = hitControl;

                // Handle enter and exit events on the GUI controlls that are hit
                HandlePointerExitAndEnter(data.pointerEvent, data.currentPoint);

                if (inputDevice.Device.InteractionInformation.IsPrimaryButtonClicked)
                {
                    ClearSelection();

                    data.pointerEvent.pressPosition = data.pointerEvent.position;
                    data.pointerEvent.pointerPressRaycast = data.pointerEvent.pointerCurrentRaycast;
                    data.pointerEvent.pointerPress = null;

                    // update current pressed if the curser is over an element
                    if (data.currentPoint != null)
                    {
                        data.currentPressed = data.currentPoint;
                    
                        data.pointerEvent.current = data.currentPressed;
                    
                        GameObject newPressed = ExecuteEvents.ExecuteHierarchy(data.currentPressed, data.pointerEvent,
                            ExecuteEvents.pointerDownHandler);
                    
                        ExecuteEvents.Execute(gameObject, data.pointerEvent, ExecuteEvents.pointerDownHandler);
                    
                        if (newPressed == null)
                        {
                            // some UI elements might only have click handler and not pointer down handler
                            newPressed = ExecuteEvents.ExecuteHierarchy(data.currentPressed, data.pointerEvent,
                                ExecuteEvents.pointerClickHandler);
                            ExecuteEvents.Execute(gameObject, data.pointerEvent,
                                ExecuteEvents.pointerClickHandler);
                            if (newPressed != null)
                            {
                                data.currentPressed = newPressed;
                            }
                        }
                        else
                        {
                            data.currentPressed = newPressed;
                            // we want to do click on button down at same time, unlike regular mouse processing
                            // which does click when mouse goes up over same object it went down on
                            // reason to do this is head tracking might be jittery and this makes it easier to click buttons
                            ExecuteEvents.Execute(newPressed, data.pointerEvent, ExecuteEvents.pointerClickHandler);
                            ExecuteEvents.Execute(gameObject, data.pointerEvent,
                                ExecuteEvents.pointerClickHandler);
                        }

                        if (newPressed != null)
                        {
                            data.pointerEvent.pointerPress = newPressed;
                            data.currentPressed = newPressed;
                            Select(data.currentPressed);
                        }

                        ExecuteEvents.Execute(data.currentPressed, data.pointerEvent, ExecuteEvents.beginDragHandler);
                        ExecuteEvents.Execute(gameObject, data.pointerEvent, ExecuteEvents.beginDragHandler);

                        data.pointerEvent.pointerDrag = data.currentPressed;
                        data.currentDragging = data.currentPressed;
                    }
                } // button down end


                if (!inputDevice.Device.InteractionInformation.IsPrimaryButtonClicked)
                {
                    if (data.currentDragging != null)
                    {
                        data.pointerEvent.current = data.currentDragging;
                        ExecuteEvents.Execute(data.currentDragging, data.pointerEvent, ExecuteEvents.endDragHandler);
                        ExecuteEvents.Execute(gameObject, data.pointerEvent, ExecuteEvents.endDragHandler);
                        if (data.currentPoint != null)
                        {
                            ExecuteEvents.ExecuteHierarchy(data.currentPoint, data.pointerEvent, ExecuteEvents.dropHandler);
                        }

                        data.pointerEvent.pointerDrag = null;
                        data.currentDragging = null;
                    }

                    if (data.currentPressed)
                    {
                        data.pointerEvent.current = data.currentPressed;
                        ExecuteEvents.Execute(data.currentPressed, data.pointerEvent, ExecuteEvents.pointerUpHandler);
                        ExecuteEvents.Execute(gameObject, data.pointerEvent, ExecuteEvents.pointerUpHandler);
                        data.pointerEvent.rawPointerPress = null;
                        data.pointerEvent.pointerPress = null;
                        data.currentPressed = null;
                    }
                }


                // drag handling
                if (data.currentDragging != null)
                {
                    data.pointerEvent.current = data.currentPressed;
                    ExecuteEvents.Execute(data.currentDragging, data.pointerEvent, ExecuteEvents.dragHandler);
                    ExecuteEvents.Execute(gameObject, data.pointerEvent, ExecuteEvents.dragHandler);
                }


                // update selected element for keyboard focus
                if (base.eventSystem.currentSelectedGameObject != null)
                {
                    data.pointerEvent.current = eventSystem.currentSelectedGameObject;
                    ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, GetBaseEventData(),
                        ExecuteEvents.updateSelectedHandler);
                    //ExecuteEvents.Execute(controller.gameObject, GetBaseEventData(), ExecuteEvents.updateSelectedHandler);
                }
            }
        }
    }
}