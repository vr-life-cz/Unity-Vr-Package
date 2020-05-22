using Vrlife.Core.Mvc;

public interface ISceneTransitorController : IController<ISceneTransitorView>
{
    void ChangeScene(string scene);
}