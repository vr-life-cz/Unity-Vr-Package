using UnityEngine.XR;

namespace Vrlife.Core.Vr
{
  public class Controller
  {
    public InputDevice device;
    public ControllerData data;

    public Controller(InputDevice inputDevice)
    {
      device = inputDevice;
      data = new ControllerData();
    }

    public void SendHaptic(float duration = 1f)
    {
      device.SendHapticImpulse(0, .5f, duration);
    }
  }
}