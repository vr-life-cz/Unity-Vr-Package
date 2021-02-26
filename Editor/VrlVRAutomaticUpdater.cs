using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using Vrlife.Core;

namespace Plugins.com.vrlife.vr.Editor
{
    public class VrlVRAutomaticUpdater
    {
        [InitializeOnLoad]
        public class AutomaticUpdate
        {
            static AutomaticUpdate()
            {
                Client.Add("https://github.com/virtual-real-life/Unity-Vr-Package.git");

                if (Client.Embed("com.vrlife.core") is null)
                {
                    Client.Add("https://github.com/virtual-real-life/Unity-Core-Package.git");
                }
            }
        }
    }
}