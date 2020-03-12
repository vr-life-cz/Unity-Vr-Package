using UnityEngine;
using Zenject;

namespace Vrlife.Core.Vr
{
    public class VrRigControllerProxy : MonoBehaviour
    {
        [Inject] public IPlayerInputUpdater inputUpdater;
        private void Update()
        {
            Vector2 input = inputUpdater.RightHandInputDevice.InteractionInformation.JoystickPosition;
            Vector3 coordinates = new Vector3(input.x, 0, input.y);
            transform.position += coordinates * 0.1f;
        }
    }
}