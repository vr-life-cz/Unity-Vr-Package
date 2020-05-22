using Vrlife.Core.Mvc;

public class SceneTransitorProcessor : IViewProcessor<SceneTransitorViewModel, ISceneTransitorView, ISceneTransitorController>
{
    public void ProcessModel(SceneTransitorViewModel model, ISceneTransitorController controller)
    {
        model.material.color = model.color;
    }
}