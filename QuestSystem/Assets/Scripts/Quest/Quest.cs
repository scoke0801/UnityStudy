using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

public enum QuestState
{ 
    Inactive,
    Running,
    Complete,
    Cancel,
    WaitingForCompletion,
}


[CreateAssetMenu(menuName = "Quest/Quest", fileName = "Quest_")]
public class Quest : ScriptableObject
{
    #region Events
    public delegate void TaskSuccessChangedHandler(Quest quest, Task task, int currentSuccess, int prevSuccess);
    public delegate void CompletedHandler(Quest quest);
    public delegate void CanceledHandler(Quest quest);
    public delegate void NewTaskGroupHandler(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup);
    #endregion
    [SerializeField]
    private Category _category;

    [SerializeField]
    private Sprite _icon;

    [Header("Text")]
    [SerializeField]
    private string _codeName;

    [SerializeField]
    private string _displayName;

    [SerializeField, TextArea]
    private string _description;

    [Header("Task")]
    [SerializeField]
    private TaskGroup[] _taskGroups;

    [Header("Reward")]
    [SerializeField]
    private Reward[] _rewards;

    [Header("Option")]
    [SerializeField]
    private bool _useAutoComplete;
    [SerializeField]
    private bool _isCancelAble;
    [SerializeField]
    private bool _isSavable;

    [Header("Condition")]
    [SerializeField]
    private Condition[] _acceptionConditions;
    [SerializeField]
    private Condition[] _cancelConditions;

    private int _currentTaskGroupIndex;
    public Category Category => _category;
    public Sprite Icon => _icon;
    public string CodeName => _codeName;

    public string DisplayName => _displayName;

    public string Description => _description;

    public QuestState State { get; private set; }
    public TaskGroup CurrentTaskGroup => _taskGroups[_currentTaskGroupIndex];

    public IReadOnlyList<TaskGroup> TaskGroups => _taskGroups;
    public IReadOnlyList<Reward> Rewards => _rewards;

    public bool IsRegistered => State != QuestState.Inactive;
    public bool IsComplatable => State == QuestState.WaitingForCompletion;

    public bool IsComplete => State == QuestState.Complete;
    public bool IsCancel => State == QuestState.Cancel;
    public virtual bool IsCancelAble => _isCancelAble && _cancelConditions.All(x => x.IsPass(this));
    public bool IsAcceptable => _acceptionConditions.All(x => x.IsPass(this));
    public virtual bool IsSavable => _isSavable;

    public event TaskSuccessChangedHandler _onTaskSuccessChanged;
    public event CompletedHandler _onCompleted;
    public event CanceledHandler _onCanceled;
    public event NewTaskGroupHandler _onNewTaskGroup;

    // 퀘스트를 등록
    public void OnRegister()
    {
        Debug.Assert(!IsRegistered, "This quest has already been registered.");
        foreach (var taskGroup in _taskGroups)
        {
            taskGroup.Setup(this);
            foreach (var task in taskGroup.Tasks)
            {
                task._onSuccessChanged += OnSuccessChanged;
            }
        }
        State = QuestState.Running;
        CurrentTaskGroup.Start();
    }

    public void ReceiveReport(string Category, object target, int successCount)
    {
        Debug.Assert(IsRegistered, "This quest has already been registered");
        Debug.Assert(!IsCancel, "This quest has been canceled");

        if (IsComplete)
        {
            return;
        }

        CurrentTaskGroup.ReceiveReport(Category, target, successCount);

        if (CurrentTaskGroup.IsAllTaskComplete)
        {
            if (_currentTaskGroupIndex + 1 == _taskGroups.Length)
            {
                State = QuestState.WaitingForCompletion;
                if (_useAutoComplete)
                {
                    Complete();
                }
            }
            else
            {
                var prevTaskGroup = _taskGroups[_currentTaskGroupIndex++];
                prevTaskGroup.End();
                CurrentTaskGroup.Start();
                _onNewTaskGroup?.Invoke(this, CurrentTaskGroup, prevTaskGroup);
            }
        }
        else
        {
            State = QuestState.Running;
        }
    }

    // 진행 중인 퀘스트를 완료
    public void Complete()
    {
        CheckIsRunning();

        foreach (var taskgroup in _taskGroups)
        {
            taskgroup.Complete();
        }

        foreach (var reward in _rewards)
        {
            reward.Give(this);
        }

        State = QuestState.Complete;

        _onCompleted?.Invoke(this);

        _onTaskSuccessChanged = null;
        _onCompleted = null;
        _onCanceled = null;
        _onNewTaskGroup = null;
    }

    // 진행 중인 퀘스트를 취소.
    public virtual void Cancel()
    {
        CheckIsRunning();

        Debug.Assert(IsCancelAble, "This quest can't be canceled");

        State = QuestState.Cancel;
        _onCanceled?.Invoke(this);
    }

    public Quest Clone()
    {
        Quest clone = Instantiate(this);
        clone._taskGroups = _taskGroups.Select(x => new TaskGroup(x)).ToArray();

        return clone;
    }

    public QuestSaveData ToSaveData()
    {
        return new QuestSaveData
        {
            codeName = _codeName,
            state = State,
            taskGroupIndex = _currentTaskGroupIndex,
            taskSuccessCounts = CurrentTaskGroup.Tasks.Select(x => x.CurrentSuccess).ToArray()
        };
    } 

    public void LoadFrom(QuestSaveData saveData)
    {
        State = saveData.state;
        _currentTaskGroupIndex = saveData.taskGroupIndex;

        // 이전에 가지고 있던 taskGroup에 대해서는 모두 complete 처리
        for(int i = 0; i < _currentTaskGroupIndex; ++i)
        {
            TaskGroup taskGroup = _taskGroups[i];
            taskGroup.Start();
            taskGroup.Complete();
        }

        for(int i = 0;i < saveData.taskSuccessCounts.Length; ++i)
        {
            CurrentTaskGroup.Start();
            CurrentTaskGroup.Tasks[i].CurrentSuccess = saveData.taskSuccessCounts[i];
        }
    }

    private void OnSuccessChanged(Task task, int currentSuccess, int prevSuccess)
        => _onTaskSuccessChanged?.Invoke(this, task, currentSuccess, prevSuccess);

    [Conditional("UNITY_EDITOR")]
    private void CheckIsRunning()
    {
        Debug.Assert(IsRegistered, "This quest has already been registered");
        Debug.Assert(!IsCancel, "This quest has been canceled");
        Debug.Assert(!IsComplete, "This quest has already benn completed");
    }
}
