using System;
using UnityEngine;
using UnityEngine.Events;
using Vrlife.Core.Mvc;
using Vrlife.Core.Mvc.Implementations;

namespace Vrlife.Core.Vr
{
    
    
    [Serializable]
    public class PlayerHandView : IPlayerHandView
    {
        [SerializeField] private ProximityWatcher watcher;
        [SerializeField] private HumanBodyPart handType;
        [SerializeField] private Transform handRootTransform;
        [SerializeField] private UnityEvent<PlayerHandInputDevice> onTriggerClicked;
        [SerializeField] private UnityEvent<PlayerHandInputDevice> onTriggerReleased;
        [SerializeField] private MonoAnimator animator;

        public ProximityWatcher Watcher => watcher;

        public HumanBodyPart HandType => handType;

        public IAnimatorComponent Animator => animator;

        public Transform HandRootTransform => handRootTransform;

        public UnityEvent<PlayerHandInputDevice> OnTriggerClicked => onTriggerClicked;

        public UnityEvent<PlayerHandInputDevice> OnTriggerReleased => onTriggerReleased;
    }
}