using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    public List<string> _nextSceneNames = new List<string>();
    public List<AsyncOperation> _loadOperations = new List<AsyncOperation>();
    private int _loadedCount = 0;

    public Image _loadingBar;
    public Button _nextButton;
    public Image _backgroundImagae;

    public float _minLoadDuration = 3.0f;

    private String _loadingSceneName;

    private float _loadRatio;
    public float LoadRatio
    {
        get { return _loadRatio; }
        set
        {
            _loadRatio = value;
            _loadingBar.fillAmount = value;
        }
    }

    public void Awake()
    {
        Init();

       //LoadNextScene();
    }
    private void Init()
    {
        LoadRatio = 0.0f;
        _loadedCount = 0;

        _nextButton.onClick.AddListener(LoadNextScene);
        //_nextButton.gameObject.SetActive(false);

        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene != null)
        {
            _loadingSceneName = activeScene.name;
        }
    }

    void AddLoadSceneName(string sceneName)
    {
        _nextSceneNames.Add(sceneName);
    }

    private void LoadNextScene()
    {
        _nextButton.gameObject.SetActive(true);

        _loadOperations.Clear();
        for(int index = 0; index < _nextSceneNames.Count; ++index)
        {
            _loadOperations.Add(new AsyncOperation());
            StartCoroutine(nameof(LoadSceneRoutine), index);
        }
    }

    private void MoveToNextScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();

        foreach (var handle in _loadOperations)
        {
            handle.allowSceneActivation = true;
        }

        StartCoroutine(nameof(MoveToNextSceneRoutine));
    }
    
    private IEnumerator MoveToNextSceneRoutine()
    {
        while (true)
        {
            int loadDoneCount = 0;

            foreach (var loadOp in _loadOperations)
            {
                if (loadOp.isDone)
                {
                    ++loadDoneCount;
                }
            }

            if (loadDoneCount == _loadOperations.Count)
            {
                break;
            }

            yield return null;
        }

        SceneManager.UnloadSceneAsync(_loadingSceneName);
    }
    private IEnumerator LoadSceneRoutine(int index)
    {
        string name = _nextSceneNames[index];

        _loadOperations[index] = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        _loadOperations[index].allowSceneActivation = false;

        float fakeLoadTime = 0f;
        float fakeLoadRatio = 0f;

        while (!_loadOperations[index].isDone)
        {
            fakeLoadTime += Time.deltaTime;
            fakeLoadRatio = fakeLoadTime / _minLoadDuration;

            LoadRatio = Mathf.Min(_loadOperations[index].progress + 0.1f, fakeLoadRatio);
            Debug.Log($"Load...{_loadOperations[index].progress}, {index}" );

            if (LoadRatio >= 1.0f)
            {
                break;
            }
            yield return null;
        }

        ++_loadedCount;
        if (_loadedCount == _nextSceneNames.Count)
        {
            LoadRatio = 1.0f;

            Debug.Log("All Scene Loaded");
            MoveToNextScene();
        }
    }
}
