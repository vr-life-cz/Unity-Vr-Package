using UnityEngine;
using Vrlife.Core.Mvc;
using Vrlife.Core.Mvc.Implementations;

public class SceneTransitorView : MonoBehaviour, ISceneTransitorView
{
    [SerializeField]
    private  MonoAnimator animatorComponent;

    public IAnimatorComponent AnimatorComponent => animatorComponent;
}