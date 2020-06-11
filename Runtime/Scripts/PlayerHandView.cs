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

    [Serializable]
    public class ControllerInputBinding
    {
        public ControllerInput input;
        public UnityEvent handler;
        public ControllerAxisHandler axisHandler;
    }

    [Serializable]
    public class ControllerAxisHandler : UnityEvent<InteractionInformation>
    {
        
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
        [SerializeField] private List<ControllerInputBinding> inputBindings;


        public ProximityWatcher Watcher => watcher;

        public HumanBodyPart HandType => handType;

        public IAnimatorComponent Animator => animator;

        public List<ControllerInputBinding> InputBindings => inputBindings;

        public Transform HandRootTransform => handRootTransform;
    }
}