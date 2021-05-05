using UnityEngine;

namespace Plugins.com.vrlife.vr.Runtime.Scripts
{
    public class AnimatorEventHandler : MonoBehaviour
    {
        public delegate void ScreenDarkenedDelegate();
        public static event ScreenDarkenedDelegate OnScreenDarkened;

        public void OnScreenDarkenedInvocation()
        {
            OnScreenDarkened?.Invoke();
        }
    }
}
