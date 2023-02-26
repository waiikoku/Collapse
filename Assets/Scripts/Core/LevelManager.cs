using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class LevelManager : Singleton<LevelManager>
{
    private Coroutine loadSceneCoroutine;
    [SerializeField] private string mainMenuScene = "Main";

    [Header("Loading")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI sceneName;

    public Action beginCallback;
    protected override void Awake()
    {
        base.Awake();
        if (isDuplicate)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void LoadMain(UnityAction<Scene, LoadSceneMode> callback = null)
    {
        LoadScene(mainMenuScene,callback);
    }

    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name, null, true);
    }

    public void LoadScene(string name, UnityAction<Scene, LoadSceneMode> callback = null, bool sameName = false)
    {
        if (loadSceneCoroutine != null)
        {
            Debug.Log("Already has loading request! Refuse to load.");
            return;
        }
        loadSceneCoroutine = StartCoroutine(LoadAsync(name,callback,sameName));
    }

    private IEnumerator LoadAsync(string sceneName, UnityAction<Scene,LoadSceneMode> sceneCallback = null,bool sameName = false)
    {
        beginCallback?.Invoke();
        canvasGroup.blocksRaycasts = true;
        LeanTween.alphaCanvas(canvasGroup, 1, 0.5f).setEase(LeanTweenType.linear);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        this.sceneName.text = sceneName;
        SceneManager.sceneLoaded += sceneCallback;
        operation.allowSceneActivation = false;
        float progress;
        while (true)
        {
            progress = Mathf.Clamp01(operation.progress / 0.9f);
            if(progress >= 1f)
            {
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        operation.allowSceneActivation = true;
        if (sameName == false)
        {
            while (SceneManager.GetActiveScene().name != sceneName)
            {
                yield return null;
            }
        }
        SceneManager.sceneLoaded -= sceneCallback;
        loadSceneCoroutine = null;
        LeanTween.alphaCanvas(canvasGroup, 0, 0.5f).setEase(LeanTweenType.linear);
        Action onFaded = delegate { canvasGroup.blocksRaycasts = false; };
        LeanTween.delayedCall(0.5f, onFaded);
    }
}
