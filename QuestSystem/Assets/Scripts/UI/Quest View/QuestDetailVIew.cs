using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestDetailVIew : MonoBehaviour
{
    [SerializeField]
    private GameObject _displayGroup;

    [SerializeField]
    private Button _cancelButton;

    [Header("Quest Description")]
    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]
    private TextMeshProUGUI _description;

    [Header("Task Description")]
    [SerializeField]
    private RectTransform _taskDescriptionGroup;
    [SerializeField]
    private TaskDescriptor _taskDescriptorPrefab;
    [SerializeField]
    private int _taskDescriptorPoolCount;

    [Header("Reward Description")]
    [SerializeField]
    private RectTransform _rewardDescriptionGroup;
    [SerializeField]
    private TextMeshProUGUI _rewardDescriptionPrefab;
    [SerializeField]
    private int _rewardDescriptionPoolCount;

    private List<TaskDescriptor> _taskDescriptorPool;
    private List<TextMeshProUGUI> _rewardDescriptorPool;

    public Quest Target { get; set; }

    private void Awake()
    {
        _taskDescriptorPool = CreatePool(_taskDescriptorPrefab, _taskDescriptorPoolCount, _taskDescriptionGroup);
        _rewardDescriptorPool = CreatePool(_rewardDescriptionPrefab, _rewardDescriptionPoolCount, _rewardDescriptionGroup);
        _cancelButton.onClick.AddListener(CancelQuest);
    }
    private List<T> CreatePool<T>(T prefab, int count, RectTransform parent)
        where T : MonoBehaviour
    {
        var pool = new List<T>(count);

        for (int i = 0; i < count; ++i)
        {
            pool.Add(Instantiate(prefab, parent));
        }
        return pool;
    }

    public void Start()
    {
        _displayGroup.SetActive(false);
    }
    private void CancelQuest()
    {
        if (Target.IsCancelAble)
        {
            Target.Cancel();
        }
    }

    public void Show(Quest quest)
    {
        _displayGroup.SetActive(true);
        Target = quest;

        _title.text = quest.DisplayName;
        _description.text = quest.Description;

        int taskIndex = 0;
        foreach (var taskGroup in quest.TaskGroups)
        {
            foreach (var task in taskGroup.Tasks)
            {
                var poolObject = _taskDescriptorPool[taskIndex++];
                poolObject.gameObject.SetActive(true);

                if (taskGroup.IsComplete)
                {
                    poolObject.UpdateTextUsingStrikeThrough(task);
                }
                else if (taskGroup == quest.CurrentTaskGroup)
                {
                    poolObject.UpdateText(task);
                }
                else
                {
                    poolObject.UpdateText("● ?????????");
                }
            }
        }

        // 사용하지 않는 풀 객체 비활성화
        for (int i = taskIndex; i < _taskDescriptorPoolCount; ++i)
        {
            _taskDescriptorPool[i].gameObject.SetActive(false);
        }

        var rewards = quest.Rewards;
        var rewardCount = rewards.Count;
        for (int i = 0; i < _rewardDescriptionPoolCount; ++i)
        {
            var poolObject = _rewardDescriptorPool[i];
            if (i < rewardCount)
            {
                var reward = rewards[i];
                poolObject.text = $"●{reward.Description} + {reward.Quantity}";
                poolObject.gameObject.SetActive(true);
            }
            else
            {
                poolObject.gameObject.SetActive(false);
            }
        }

        _cancelButton.gameObject.SetActive(quest.IsCancelAble && !quest.IsComplete);
    }

    public void Hide()
    {
        Target = null;
        _displayGroup.SetActive(false);
        _cancelButton.gameObject.SetActive(false);
    }

}
