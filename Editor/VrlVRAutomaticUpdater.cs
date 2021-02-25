using UnityEditor;
using UnityEditor.PackageManager;

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
            }
        }
    }
}
