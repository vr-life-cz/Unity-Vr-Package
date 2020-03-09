using UnityEngine;
using UnityEngine.Events;
using Vrlife.Core.Mvc;

namespace Vrlife.Core.Vr
{
    public interface IPlayerHandView
    {
        ProximityWatcher Watcher { get;}
        
        HumanBodyPart HandType { get;  }

        IAnimatorComponent Animator { get; }
        
        Transform HandRootTransform { get;  }

        UnityEvent<PlayerHandInputDevice> OnTriggerClicked { get; }
        
        UnityEvent<PlayerHandInputDevice> OnTriggerReleased { get; }
    }
}