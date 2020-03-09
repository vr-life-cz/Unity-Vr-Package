using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using Vrlife.Core.Mvc;
using Vrlife.Core.Mvc.Implementations;
using Zenject;

namespace Vrlife.Core.Vr
{
    [Serializable]
    public class PlayerHandEventHandler : UnityEvent<PlayerHandInputDevice>
    {
    }

    public class PlayerHandViewProcessor : IViewProcessor<PlayerHandViewModel, IPlayerHandView, IPlayerHandController>
    {
        public void ProcessModel(PlayerHandViewModel model, IPlayerHandController controller)
        {
            
        }
    }

    public class PlayerHandControllerProxy : MonoViewControllerProxy<PlayerHandViewModel, IPlayerHandView, IPlayerHandController>, IPlayerHandController, IDebugInfoProvider
    {
        [Inject] public IPlayerInputUpdater inputUpdater;

        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public string Label => gameObject.name;

        public string GetDebugInfo()
        {
            _stringBuilder.Clear();

            var LeftHand = View.HandType == HumanBodyPart.LeftHand
                ? inputUpdater.LeftHandInputDevice.InteractionInformation
                : inputUpdater.RightHandInputDevice.InteractionInformation;

            if (LeftHand != null)
            {
                _stringBuilder.AppendLine($"Inputs");
                _stringBuilder.AppendLine(
                    $"Trigger - touched {LeftHand.IsTriggerTouched} | pressure {LeftHand.TriggerPressure}");
                _stringBuilder.AppendLine(
                    $"Grip - touched {LeftHand.IsGripTouched} | pressure {LeftHand.GripPressure}");
                _stringBuilder.AppendLine(
                    $"Joystick - touched {LeftHand.IsJoystickTouched} | clicked {LeftHand.IsJoystickClicked} | position {LeftHand.JoystickPosition}");
            }

            return _stringBuilder.ToString();
        }


        [SerializeField]
        private PlayerHandView view;

        protected override IPlayerHandView View => view;

        public void Update()
        {
            Controller.Update();
        }
    }
}