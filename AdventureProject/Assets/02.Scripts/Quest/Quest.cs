using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public delegate void TaskSuccessChangedHandler(Quest quest, QuestTask task, int currentSuccess, int prevSuccess);
    public delegate void CompletedHandler(Quest quest);
    public delegate void CanceledHandler(Quest quest);
    public delegate void NewTaskGroupHandler(Quest quest, QuestTaskGroup currentTaskGroup, QuestTaskGroup prevTaskGroup);
    #endregion

    #region Variables
    public event TaskSuccessChangedHandler _onTaskSuccessChanged;
    public event CompletedHandler _onCompleted;
    public event CanceledHandler _onCanceled;
    public event NewTaskGroupHandler _onNewTaskGroup;

    [SerializeField] private QuestCategory _category;

    [SerializeField] private Sprite _icon;

    [Header("Text")]
    [SerializeField] private string _codeName;

    [SerializeField] private string _displayName;

    [SerializeField, TextArea] private string _description;

    [Header("Task")]
    [SerializeField] private QuestTaskGroup[] _taskGroups;

    [Header("Reward")]
    [SerializeField] private QuestReward[] _rewards;

    [Header("Option")]
    [SerializeField] private bool _useAutoComplete;
    [SerializeField] private bool _isCancelAble;
    [SerializeField] private bool _isSavable;

    [Header("Condition")]
    [SerializeField] private QuestCondition[] _acceptionConditions;
    [SerializeField] private QuestCondition[] _cancelConditions;

    private int _currentTaskGroupIndex;
    #endregion

    #region Properties
    public QuestCategory Category => _category;
    public Sprite Icon => _icon;
    public string CodeName => _codeName;

    public string DisplayName => _displayName;

    public string Description => _description;

    public QuestState State { get; private set; }
    public QuestTaskGroup CurrentTaskGroup => _taskGroups[_currentTaskGroupIndex];

    public IReadOnlyList<QuestTaskGroup> TaskGroups => _taskGroups;
    public IReadOnlyList<QuestReward> Rewards => _rewards;

    public bool IsRegistered => State != QuestState.Inactive;
    public bool IsComplatable => State == QuestState.WaitingForCompletion;

    public bool IsComplete => State == QuestState.Complete;
    public bool IsCancel => State == QuestState.Cancel;
    public virtual bool IsCancelAble => _isCancelAble && _cancelConditions.All(x => x.IsPass(this));
    public bool IsAcceptable => _acceptionConditions.All(x => x.IsPass(this));
    public virtual bool IsSavable => _isSavable;
    #endregion

    #region Unity Methods
    #endregion

    #region Public Methods
    // 퀘스트를 등록
    public void OnRegister()
    {
        Debug.Assert(!IsRegistered, "This quest has already been registered.");
        foreach (var taskGroup in _taskGroups)
        {
            taskGroup.Setup(this);
            foreach (var task in taskGroup.Task)
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

        if (CurrentTaskGroup.IsAllTaskComplete())
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
        Debug.Assert(IsCancelAble, "This quest can't be canceled");

        State = QuestState.Cancel;
        _onCanceled?.Invoke(this);
    }

    public bool ContainsTarget(object target) => _taskGroups.Any(x => x.ContainsTarget(target));

    public bool ContainsTarget(QuestTaskTarget target) => ContainsTarget(target.Value);
    public Quest Clone()
    {
        Quest clone = Instantiate(this);
        clone._taskGroups = _taskGroups.Select(x => new QuestTaskGroup(x)).ToArray();

        return clone;
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void OnSuccessChanged(QuestTask task, int currentSuccess, int prevSuccess)
        => _onTaskSuccessChanged?.Invoke(this, task, currentSuccess, prevSuccess);
    #endregion
}
