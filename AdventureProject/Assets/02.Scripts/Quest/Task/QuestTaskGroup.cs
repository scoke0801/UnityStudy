using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum TaskGroupState
{ 
    Inactive,
    Running,
    Complete,
}

[System.Serializable]
public class QuestTaskGroup
{
    #region Variables
    [SerializeField]
    private QuestTask[] _tasks;
    #endregion

    #region Properties
    public IReadOnlyList<QuestTask> Task => _tasks;
    public Quest Owner { get; private set; }
    public bool IsComplete => State == TaskGroupState.Complete;
    public TaskGroupState State { get; private set; }
    #endregion
    
    #region Public Methods
    public QuestTaskGroup(QuestTaskGroup copyTarget)
    {
        _tasks = copyTarget.Task.Select(x => Object.Instantiate(x)).ToArray();
    }
    public void Setup(Quest owner)
    {
        Owner = owner;
        for( int i = 0; i < _tasks.Length; ++i)
        {
            _tasks[i].Setup(owner);
        }
    }

    public void Start()
    {
        State = TaskGroupState.Running;
        for(int i = 0; i < _tasks.Length; ++i)
        {
            _tasks[i].Start();
        }
    }

    public void End()
    {
        for(int i = 0; i < _tasks.Length; ++i)
        {
            _tasks[i].End();
        }
    }
    public void ReceiveReport(string category, object target, int successCount)
    {
        foreach (var task in _tasks)
        {
            if (task.IsTarget(category, target))
            {
                task.RecieveReport(successCount);
            }
        }
    }
    public void Complete()
    {
        if (IsComplete)
        {
            return;
        }

        State = TaskGroupState.Complete;
        for(int i = 0; i < _tasks.Length; ++i)
        {
            QuestTask task = _tasks[i];
            if (task.IsComplete) { continue; }

            task.Complete();
        }
    }
    public bool IsAllTaskComplete()
    {
        for(int i = 0; i < _tasks.Length; ++i)
        {
            if (!_tasks[i].IsComplete)
            {
                return false;
            }
        }

        return true;
    }
    public QuestTask FindTaskByTarget(object target)
    {
        for(int i = 0; i < _tasks.Length; ++i)
        {
            if (_tasks[i].ContainsTarget(target))
            {
                return _tasks[i];
            }
        }

        return null;
    }

    public QuestTask FindTaskByTarget(QuestTaskTarget target)
    {
        return FindTaskByTarget(target.Value);
    }

    public bool ContainsTarget(object target)
    {
        for(int i = 0; i < _tasks.Length; ++i)
        {
            if (_tasks[i].ContainsTarget(target))
            {
                return true;
            }
        }

        return false;
    }

    public bool ContainsTarget(QuestTaskTarget target)
    {
        return ContainsTarget(target.Value);
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion

}