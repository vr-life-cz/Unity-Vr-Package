using UnityEngine;
using UnityEngine.SceneManagement;
using Vrlife.Core.Mvc;
using Vrlife.Core.Mvc.Implementations;

public class SceneTransitorController : ISceneTransitorController
{
    private ISceneTransitorView _view;

    private AsyncOperation _loading;
    
    private readonly ICoroutineProcessor _coroutineProcessor;

    public SceneTransitorController(ICoroutineProcessor coroutineProcessor)
    {
        _coroutineProcessor = coroutineProcessor;
    }

    public void BindView(ISceneTransitorView view)
    {
        _view = view;
    }

    public void ChangeScene(string scene)
    {
        _loading = SceneManager.LoadSceneAsync(scene);
        
        _loading.allowSceneActivation = false;
        
        _view.AnimatorComponent.SetTrigger(Animator.StringToHash("fadeout"));

        _coroutineProcessor.Build(() => { }).WaitForSeconds(2).Then(() =>
        {
            _loading.allowSceneActivation = true;
        }).Execute();
    }
}