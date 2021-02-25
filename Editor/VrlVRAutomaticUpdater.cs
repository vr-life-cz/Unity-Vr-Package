using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Plugins.com.vrlife.vr.Editor
{
    public class VrlVRAutomaticUpdater
    {
        [InitializeOnLoad]
        public class AutomaticUpdate
        {
            static AutomaticUpdate()
            {
                try
                {
                    Client.Add("https://github.com/virtual-real-life/Unity-Vr-Package.git");
                }
                catch
                {
                    Debug.Log("VRL VR package already exists.");
                }
            }
        }
    }
}