using Zenject;

namespace Vrlife.Core.Vr
{
    public class PlayerHandInteractionController : ITickable
    {
        private IPlayerInputUpdater _playerInputUpdater;

        private XrGeneralSettings _generalSettings;


        public void Tick()
        {
        }
    }
}