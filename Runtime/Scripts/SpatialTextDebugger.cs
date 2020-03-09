using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace Vrlife.Core.Vr
{
    public class SpatialTextDebugger : MonoBehaviour
    {
        private StringBuilder _stringBuilder;
        
        private IDebugInfoProvider[] _debugInfoProviders;
        
        public TextMeshPro label;

        public bool isPrintingDebug = true;
        public GameObject provider;
        private void Start()
        {
            _debugInfoProviders = provider.GetComponents<MonoBehaviour>()?.Union(provider.GetComponentsInChildren<MonoBehaviour>()).OfType<IDebugInfoProvider>().ToArray();
            _stringBuilder = new StringBuilder();
        }

        private void Update()
        {
            if (!isPrintingDebug) return;
            
            Print();
        }

        public void Print()
        {
            _stringBuilder.Clear();
            
            
            if(_debugInfoProviders == null || _debugInfoProviders.Length == 0) return;
            foreach (var provider in _debugInfoProviders)
            {
                _stringBuilder.AppendLine($"----- {provider.Label} -----");
                _stringBuilder.AppendLine(provider.GetDebugInfo());
            }

            label.text = _stringBuilder.ToString();
        }
    }
}