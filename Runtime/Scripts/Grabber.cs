using System.Linq;
using UnityEngine;
using Zenject;

namespace Vrlife.Core.Vr
{
    public class Grabber : MonoBehaviour, IDebugInfoProvider
    {
        public ProximityWatcher proximityWatcher;

        public Grabbable grabbedObject;

        [Inject] private IGrabService _grabService;

        public void Grab()
        {
            _grabService.Grab(this);
        }

        public void Release()
        {
            _grabService.Release(this);
        }

        public string GetDebugInfo()
        {
            return $"Proximity Obj - {proximityWatcher.ProximityObjects.FirstOrDefault()}\nGrabbed - {grabbedObject}";
        }

        public string Label => gameObject.name;
    }
}