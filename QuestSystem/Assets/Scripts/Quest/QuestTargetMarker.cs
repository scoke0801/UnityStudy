using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestTargetMarker : MonoBehaviour
{
    [SerializeField]
    private TaskTarget _target;

    [SerializeField]
    private MarkerMaterialData[] _markerMaterialDatas;

    private Dictionary<Quest, Task> _targetTaskByQuest = new Dictionary<Quest, Task>();
    private Transform _cameraTransform; // marker가 플레이어를 바라보게 하도록 하기 위하여
    private Renderer _renderer; // Task마다 다른 이미지를 보여주기 위한 렌더러

    private int _currentRuunninTargetTaskCount;

    [System.Serializable]
    private struct MarkerMaterialData
    {
        public Category category;
        public Material markerMaterial;
    }

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        _renderer = GetComponent<Renderer>();
    }

    private void Start()
    { 
        gameObject.SetActive(false);

        QuestSystem.Instance._onQuestRegistered += TryAddTargetQuests;
        foreach (var quest in QuestSystem.Instance.ActiveQuests)
        {
            TryAddTargetQuests(quest);
        }
    }

    private void OnDestroy()
    {
        QuestSystem.Instance._onQuestRegistered -= TryAddTargetQuests;
        foreach((Quest quest, Task task) in _targetTaskByQuest)
        {
            quest._onNewTaskGroup -= UpdateTargetTask;
            quest._onCompleted -= RemoveTargetQuest;
            task._onStateChanged -= UpdateRunningTargetTaskCount;
        }
    }
    private void Update()
    {
        var rotation = Quaternion.LookRotation((_cameraTransform.position - transform.position).normalized);
        transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y + 180.0f, 0);
    }
    private void TryAddTargetQuests(Quest quest)
    {
        if(_target != null && quest.ContainsTarget(_target))
        {
            quest._onNewTaskGroup += UpdateTargetTask;
            quest._onCompleted += RemoveTargetQuest;

            UpdateTargetTask(quest, quest.CurrentTaskGroup);
        }
    }

    private void UpdateTargetTask(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup = null)
    {
        _targetTaskByQuest.Remove(quest);

        var task = currentTaskGroup.FindTaskByTarget(_target);
        if (task != null)
        {
            _targetTaskByQuest[quest] = task;
            task._onStateChanged += UpdateRunningTargetTaskCount;

            UpdateRunningTargetTaskCount(task, task.CurState);
        }
    }

    private void RemoveTargetQuest(Quest quest) => _targetTaskByQuest.Remove(quest);

    private void UpdateRunningTargetTaskCount(Task task, TaskState currentState, TaskState prevState = TaskState.Inactive)
     {
        if (currentState == TaskState.Runnuing)
        {
            _renderer.material = _markerMaterialDatas.First(x => x.category == task.Category).markerMaterial;
            _currentRuunninTargetTaskCount++;
        }
        else
        {
            _currentRuunninTargetTaskCount--;
        }
        gameObject.SetActive(_currentRuunninTargetTaskCount != 0);
    } 
}
