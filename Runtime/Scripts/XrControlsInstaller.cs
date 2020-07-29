using Vrlife.Core.Mvc;
using Vrlife.Core.Mvc.Implementations;
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
            
            Container.Bind<IGrabService>().To<GrabService>().AsSingle();

            Container.BindViewController<IPlayerHandView, IPlayerHandController>()
                .WithControllerImplementation<PlayerHandController>().WithViewModelProcessor<PlayerHandViewModel, PlayerHandViewProcessor>();
            
            
            Container.BindViewController<ISceneTransitorView, ISceneTransitorController>()
                .WithControllerImplementation<SceneTransitorController>()
                .WithViewModelProcessor<SceneTransitorViewModel, SceneTransitorProcessor>();
            
            Container.BindInterfacesAndSelfTo<CoroutineProcessor>().FromNewComponentOnNewGameObject().AsSingle();
        }
    }
}