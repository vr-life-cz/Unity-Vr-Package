using Vrlife.Core;
using Vrlife.Core.GoogleApis.TextToSpeech;
using Vrlife.Core.Mvc;
using Vrlife.Core.Mvc.Implementations;
using Zenject;

public class DummyInstaller : MonoInstaller<DummyInstaller>
{
    public override void InstallBindings()
    {
        Container.BindViewController<ISimpleView, ISimpleController>()
            .WithControllerImplementation<SimpleController>()
            .WithViewModelProcessor<SimpleViewModel, SimpleViewProcessor>();

        Container.Bind<IGoogleTextToSpeechService>().To<GoogleTextToSpeechService>().AsSingle();

        Container.BindInterfacesAndSelfTo<CoroutineProcessor>().FromNewComponentOnNewGameObject().AsSingle();

        Container.BindViewController<ISceneTransitorView, ISceneTransitorController>()
            .WithControllerImplementation<SceneTransitorController>()
            .WithViewModelProcessor<SceneTransitorViewModel, SceneTransitorProcessor>();
    }
}