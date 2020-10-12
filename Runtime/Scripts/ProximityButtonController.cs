using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Vrlife.Core;
using Button = UnityEngine.UI.Button;

public class ProximityButtonController : MonoBehaviour
{
    public Renderer loadingRenderer;
    [Range(0, 1)] /*[ReadOnly]*/ public float progress;
    public float speedMultiplier = 1f;
    public GameObject button;
    private static readonly int _shaderProgress = Shader.PropertyToID("_Progress");
    private static readonly int _shaderIsPulsating = Shader.PropertyToID("_IsPulsating");
    public UnityEvent onClick;

    [ReadOnly] public bool isTouching;
    public bool isPulsating;

    [InlineButton("SwitchDisabled", "Switch isDisabled")]
    public bool buttonDisabled;

    [InlineButton("SwitchHidden", "Switch isHidden")]
    public bool buttonHidden;

    public void TriggerEnter(ProximityWatcher proximityWatcher, Collider collider)
    {
        if (collider.isTrigger)
        {
            isTouching = true;
        }
    }

    public void TriggerExit(ProximityWatcher proximityWatcher, Collider collider)
    {
        if (collider.isTrigger)
        {
            isTouching = false;
        }
    }

    public void SwitchHidden()
    {
        buttonHidden = !buttonHidden;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(!buttonHidden);
        }
    }

    public void SetHidden(bool value)
    {
        buttonHidden = value;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(!buttonHidden);
        }
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        SetHidden(!active);
    }

    public void SwitchDisabled()
    {
        buttonDisabled = !buttonDisabled;
        button.gameObject.SetActive(!buttonDisabled);
    }

    private void Update()
    {
        progress = isTouching
            ? Mathf.Clamp01(progress + speedMultiplier * Time.deltaTime)
            : Mathf.Clamp01(progress - speedMultiplier * Time.deltaTime);


        if (Mathf.Approximately(progress, 1))
        {
            onClick?.Invoke();
            SetHidden(true);
            isTouching = false;
            progress = 0;
        }

        loadingRenderer.material.SetFloat(_shaderProgress, progress);
        loadingRenderer.material.SetInt(_shaderIsPulsating, isPulsating ? 1 : 0);
    }
}