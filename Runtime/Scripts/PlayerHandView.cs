using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vrlife.Core.Mvc;
using Vrlife.Core.Mvc.Implementations;

namespace Vrlife.Core.Vr
{
    [Serializable]
    public class PlayerHandInputDeviceEventHandler : UnityEvent<PlayerHandInputDevice>
    {
    }

    public class ControllerInputBinding
    {
        public ControllerInput input;
        public UnityEvent[] handlers;
    }

    [Serializable]
    public class PlayerHandView : IPlayerHandView
    {
        [SerializeField] private ProximityWatcher watcher;
        [SerializeField] private HumanBodyPart handType;

        [SerializeField] private Transform handRootTransform;

//        [SerializeField] private PlayerHandInputDeviceEventHandler onTriggerClicked;
//        [SerializeField] private PlayerHandInputDeviceEventHandler onTriggerReleased;
        [SerializeField] private MonoAnimator animator;
        [SerializeField] private Grabber grabber;
        [SerializeField] private List<ControllerInputBinding> inputBindings;


        public ProximityWatcher Watcher => watcher;

        public HumanBodyPart HandType => handType;
        public Grabber Grabber => grabber;

        public IAnimatorComponent Animator => animator;

        public List<ControllerInputBinding> InputBindings => inputBindings;

        public Transform HandRootTransform => handRootTransform;

//        public UnityEvent<PlayerHandInputDevice> OnTriggerClicked => onTriggerClicked;
//
//        public UnityEvent<PlayerHandInputDevice> OnTriggerReleased => onTriggerReleased;
    }
}