using Vrlife.Core.Mvc;
using Zenject;

namespace Vrlife.Core.Vr
{
    public class XrControlsInstaller : MonoInstaller
    {
        public bool enableVRInput;
        
        public XrGeneralSettings GeneralSettings;
        
        public override void InstallBindings()
        {
            
#if !UNITY_EDITOR
            enableVRInput = true;
#endif
            
            if (!enableVRInput)
            {
                Container.Bind<IPlayerInputUpdater>().To<VrFallbackContolUpdater>().FromNewComponentOnNewGameObject()
                    .AsSingle();
            }
            else
            {
                Container.Bind<IPlayerInputUpdater>().To<VrControlUpdater>().FromNewComponentOnNewGameObject()
                    .AsSingle();
            }

            Container.Bind<XrGeneralSettings>().FromScriptableObject(GeneralSettings).AsSingle();
            
            Container.Bind<IPlayerHandInteractionService>().To<PlayerHandInteractionService>().AsSingle();

            Container.Bind<IGrabService>().To<GrabService>().AsSingle();

            Container.BindViewController<IPlayerHandView, IPlayerHandController>()
                .WithControllerImplementation<PlayerHandController>().WithViewModelProcessor<PlayerHandViewModel, PlayerHandViewProcessor>();
        }
    }
}