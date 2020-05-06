using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;



public class ScenePortal : MonoBehaviour
{
    public Animator animator;
    private AsyncOperation _loading;

    public void ChangeScene(string scene)
    {
        _loading = SceneManager.LoadSceneAsync(scene);
        _loading.allowSceneActivation = false;
        animator.SetTrigger(Animator.StringToHash("fadeout"));
        
        StartCoroutine(SceneSwitchDelay());
    }

    public IEnumerator SceneSwitchDelay()
    {
        yield return new WaitForSeconds(2);
        
        _loading.allowSceneActivation = true;
    }
}
