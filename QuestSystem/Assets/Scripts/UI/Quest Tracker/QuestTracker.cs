using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestTracker : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _questTitleText;

    [SerializeField]
    private TaskDescriptor _taskDescriptorPrefab;

    private Dictionary<Task, TaskDescriptor> _taskDescriptorByTask = new Dictionary<Task, TaskDescriptor>();

    private Quest _targetQuest;

    private void OnDestroy()
    {
        if(_targetQuest != null)
        {
            _targetQuest._onNewTaskGroup -= UpdateTaskDescriptors;
            _targetQuest._onCompleted -= DestroySelf;
        }

        foreach(var tuple in _taskDescriptorByTask)
        {
            var task = tuple.Key;
            task._onSuccessChanged -= UpdateText;
        }
    }
    public void Setup(Quest targetQuest, Color titleColor)
    {
        _targetQuest = targetQuest;
        _questTitleText.text = targetQuest.Category == null ?
            targetQuest.DisplayName :
            $"[{targetQuest.Category.DisplayName}] {targetQuest.DisplayName}";

        _questTitleText.color = titleColor;

        // 이벤트 등록. QuestTracker 객체 삭제 시, 이벤트 제거해줘야 함
        targetQuest._onNewTaskGroup += UpdateTaskDescriptors;
        targetQuest._onCompleted += DestroySelf;

        var taskGroups = targetQuest.TaskGroups;
        UpdateTaskDescriptors(targetQuest, taskGroups[0]);

        if(taskGroups[0] != targetQuest.CurrentTaskGroup)
        {
            for(int i = 1; i < taskGroups.Count; ++i)
            {
                var taskGroup = taskGroups[i];
                UpdateTaskDescriptors(targetQuest, taskGroup, taskGroups[i - 1]);

                if(taskGroup == targetQuest.CurrentTaskGroup)
                {
                    break;
                }
            }
        }
    }

    private void UpdateTaskDescriptors(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup = null)
    {
        foreach (var task in currentTaskGroup.Tasks)
        {
            var taskDescriptor = Instantiate(_taskDescriptorPrefab, transform);
            taskDescriptor.UpdateText(task);
            task._onSuccessChanged += UpdateText;

            _taskDescriptorByTask.Add(task, taskDescriptor);
        }

        if(prevTaskGroup != null)
        {
            foreach(var task in prevTaskGroup.Tasks)
            {
                var taskDescriptor = _taskDescriptorByTask[task];
                taskDescriptor.UpdateTextUsingStrikeThrough(task);
            }
        }
    }
    private void UpdateText(Task task, int currentSuccess, int prevSuccess )
    {
        _taskDescriptorByTask[task].UpdateText(task);
    }

    private void DestroySelf(Quest quest)
    {
        Destroy(gameObject);
    }
}
