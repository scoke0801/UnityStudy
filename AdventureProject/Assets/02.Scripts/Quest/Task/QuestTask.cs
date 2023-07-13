using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public enum TaskState
{
    Inactive,
    Runnuing,
    Complete,
}

[CreateAssetMenu(menuName = "Quest/Task/Task", fileName = "QuestTaks_")]
public class QuestTask : ScriptableObject
{
    #region Delegate
    public delegate void StateChangedHanlder(QuestTask task, TaskState currentState, TaskState prevState);
    public delegate void SuccessChangedHandler(QuestTask task, int currentSuccess, int prevSuccess);
    #endregion

    #region Variables
    public event StateChangedHanlder _onStateChanged;
    public event SuccessChangedHandler _onSuccessChanged;

    [Header("Category")]
    [Tooltip("Task�� ī�װ�")]
    [SerializeField] private QuestCategory _category;

    [Tooltip("Task�� �����ϱ� ���� �̸�")]
    [Header("Text")]
    [SerializeField] private string _codeName;

    [Tooltip("Task�� ���� ����")]
    [SerializeField] private string _description;

    [Header("Action")]
    [Tooltip("Task�� ���� ó���ϴ� ���")]
    [SerializeField] private QuestTaskAction _action;

    [Header("Setting")]
    [Tooltip("Task �ʱ� ���� ��")]
    [SerializeField] private InitialSuccessValue _initialSuccessValue;
    [SerializeField] private int _needSuccessToComplete;
    [Tooltip("Task�� �Ϸ�Ǿ�� ��� ������ ���� �� ���θ� ����")]
    [SerializeField] bool _isCanRecieveReportDuringCompletion;

    [Header("Target")]
    [Tooltip("Task ���")]
    [SerializeField] private QuestTaskTarget[] _targets;

    private int _currentSuccess;
    private TaskState _taskState;
    #endregion

    #region Properties
    public string CodeName => _codeName;
    public string Description => _description;
    public int NeedSuccessToComplete => _needSuccessToComplete;
    public QuestCategory Category => _category;

    public int CurrentSuccess
    {
        get => _currentSuccess;
        set
        {
            int prevSuccess = _currentSuccess;
            _currentSuccess = Mathf.Clamp(value, 0, _needSuccessToComplete);
            if(_currentSuccess != prevSuccess)
            {
                _taskState = (_currentSuccess == _needSuccessToComplete) ? TaskState.Complete : TaskState.Runnuing;
                _onSuccessChanged?.Invoke(this, _currentSuccess, prevSuccess);
            }
        }
    }
    public TaskState CurState
    {
        get => _taskState;
        set
        {
            TaskState prevState = _taskState;
            _taskState = value;
            if(prevState != _taskState)
            {
                _onStateChanged?.Invoke(this, _taskState, prevState);
            }
        }
    }
    public bool IsComplete => CurState == TaskState.Complete;

    public Quest Owner { get; private set; }
    #endregion

    #region Public Methods
    public void Start()
    {
        _taskState = TaskState.Runnuing;
        if (_initialSuccessValue)
        {
            CurrentSuccess = _initialSuccessValue.GetValue(this);
        }
    }
    public void End()
    {
        _onStateChanged = null;
        _onSuccessChanged = null;
    }

    public void Setup(Quest owner)
    {
        Owner = owner;
    }
    public void Complete()
    {
        CurrentSuccess = _needSuccessToComplete;
    }
    public void RecieveReport(int successCount)
    {
        CurrentSuccess = _action.Run(this, _currentSuccess, successCount);
    }

    public bool IsTarget(string category, object target)
    {
        if(Category != category) { return false; }

        if ( IsComplete && !_isCanRecieveReportDuringCompletion){ return false; }

        for(int i =0; i < _targets.Length; ++i)
        {
            if (_targets[i].IsEqual(target))
            {
                return true;
            }
        }

        return false;
    }

    public bool ContainsTarget(object target)
    {
        for(int i = 0; i < _targets.Length; ++i)
        {
            if (_targets[i].IsEqual(target))
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion
}
