using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TaskState
{ 
    Inactive,
    Runnuing,
    Complete,
}

[CreateAssetMenu(menuName = "Quest/Task/Task", fileName = "Task_")]
public class Task : ScriptableObject
{
    #region Events
    public delegate void StateChangedHanlder(Task task, TaskState currentState, TaskState prevState);
    public delegate void SuccessChangedHandler(Task task, int currentSuccess, int prevSuccess);
    #endregion

    [Header("Category")]
    [Tooltip("Task의 카테고리")]
    [SerializeField]
    private Category _category;

    [Tooltip("Task를 구분하기 위한 이름")]
    [Header("Text")]
    [SerializeField]
    private string _codeName;

    [Tooltip("Task에 대한 설명")]
    [SerializeField]
    private string _description;

    [Header("Action")]
    [Tooltip("Task에 대해 처리하는 모듈")]
    [SerializeField]
    private TaskAction _action;

    [Header("Setting")]
    [Tooltip("Task 초기 성공 값")]
    [SerializeField]
    private InitialSuccessValue _initialSuccessValue;
    [SerializeField]
    private int _needSuccessToComplete;
    [Tooltip("Task가 완료되었어도 계속 응답을 받을 지 여부를 설정")]
    [SerializeField]
    private bool _isCanRecieveReportDuringCompletion;

    [Header("Target")]
    [Tooltip("Task 대상")]
    [SerializeField]
    private TaskTarget[] _targets;

    private int _currentSuccess;
    private TaskState _taskState;
    
    public event StateChangedHanlder _onStateChanged;
    public event SuccessChangedHandler _onSuccessChanged;
    public int CurrentSuccess
    {
        get => _currentSuccess;
        set
        {
            int prevSuccess = _currentSuccess;
            _currentSuccess = Mathf.Clamp(value, 0, _needSuccessToComplete);
            if(_currentSuccess != prevSuccess)
            {
                _taskState = _currentSuccess == _needSuccessToComplete ? TaskState.Complete : TaskState.Runnuing;
                _onSuccessChanged?.Invoke(this, _currentSuccess, prevSuccess);
            }
        }
    }
    public string CodeName => _codeName;
    public string Description => _description;
    public int NeedSuccessToComplete => _needSuccessToComplete;
    public Category Category => _category;

    public TaskState CurState
    {
        get => _taskState;
        set
        {
            TaskState prevState = _taskState;
            _taskState = value;
            _onStateChanged?.Invoke(this, value, _taskState);
        }
    }

    public bool IsComplete => CurState == TaskState.Complete;

    public Quest Owner { get; private set; }
    public void Setup(Quest owner)
    {
        Owner = owner;
    }
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
    public void Complete()
    {
        CurrentSuccess = _needSuccessToComplete;
    }


    public void RecieveReport(int successCount)
    {
        CurrentSuccess = _action.Run(this, _currentSuccess, successCount);
    }

    public bool IsTarget(string category, object target)
        =>  this.Category == category  && 
        _targets.Any(x => x.IsEqual(target)) &&
        ( !IsComplete || IsComplete && _isCanRecieveReportDuringCompletion );


    public bool ContainsTarget(object target) => _targets.Any(x => x.IsEqual(target));
}