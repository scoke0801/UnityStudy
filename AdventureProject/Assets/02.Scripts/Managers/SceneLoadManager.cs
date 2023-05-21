using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    public string _nextSceneName;

    public Image _loadingBar;
    public Button _loadButton;
    public Button _nextButton;

    public float _minLoadDuration = 4.0f;

    private AsyncOperation _loadOperation;

    private bool _isLoadCompleted = false;
    public bool IsLoadCompleted
    {
        get { return _isLoadCompleted; }
        set
        {
            _isLoadCompleted = value;
            // TODO. touch 동작이든 어떤 동작이든 다음 씬으로 넘어가는 상호작용을 할 수 있도록 flag 설정.
        }
    }

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
    }
    private void Init()
    {
        IsLoadCompleted = false;
        LoadRatio = 0.0f;

        _loadButton.onClick.AddListener(LoadNextScene);
        _nextButton.onClick.AddListener(MoveToNextScene);
        _nextButton.gameObject.SetActive(false);
    }

    private void LoadNextScene()
    {
        _loadButton.gameObject.SetActive(false);
        _nextButton.gameObject.SetActive(true);
        StartCoroutine(nameof(LoadSceneRoutine));
    }

    private void MoveToNextScene()
    {
        _loadOperation.allowSceneActivation = true;
    }
    
    private IEnumerator LoadSceneRoutine()
    {
        _loadOperation = SceneManager.LoadSceneAsync(_nextSceneName);
        _loadOperation.allowSceneActivation = false;

        float fakeLoadTime = 0f;
        float fakeLoadRatio = 0f;

        while (!_loadOperation.isDone)
        {
            fakeLoadTime += Time.deltaTime;
            fakeLoadRatio = fakeLoadTime / _minLoadDuration;

            LoadRatio = Mathf.Min(_loadOperation.progress + 0.1f, fakeLoadRatio);
            Debug.Log("Load.." + _loadOperation.progress);

            if(LoadRatio >= 1.0f)
            {
                break;
            }

            yield return null;
        }

        LoadRatio = 1.0f;
        IsLoadCompleted = true;
    }
}
