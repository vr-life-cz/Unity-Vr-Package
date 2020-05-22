using UnityEngine;
using Vrlife.Core.Mvc.Implementations;

public class SceneTransitorControllerProxy : MonoViewControllerProxy
    <SceneTransitorViewModel, ISceneTransitorView, ISceneTransitorController>
{
   
    [SerializeField] private SceneTransitorView view;

    protected override ISceneTransitorView View => view;


    public void ChangeScene(string scene)
    {
        Controller.ChangeScene(scene);
    }
}