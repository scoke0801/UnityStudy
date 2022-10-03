using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementDetailView : MonoBehaviour
{
    [SerializeField]
    private Image _achievementIcon;

    [SerializeField]
    private TextMeshProUGUI _titleText;

    [SerializeField]
    private TextMeshProUGUI _description;

    [SerializeField]
    private Image _rewardIcon;

    [SerializeField]
    private TextMeshProUGUI _rewardText;

    [SerializeField]
    private GameObject _completionScreen;

    private Quest _target;

    public void Setup(Quest achievement)
    {
        _target = achievement;

        _achievementIcon.sprite = achievement.Icon;
        _titleText.text = achievement.DisplayName;

        var task = achievement.CurrentTaskGroup.Tasks[0];
        _description.text = BuildTaskDescription(task);


        var reward = achievement.Rewards[0];
        _rewardIcon.sprite = reward.Icon;
        _rewardText.text = $"{reward.Description} + {reward.Quantity}";

        if(achievement.IsComplete)
        {
            _completionScreen.SetActive(true);
        }
        else
        {
            _completionScreen.SetActive(false);
            achievement._onTaskSuccessChanged += UpdateDescription;
            achievement._onCompleted += ShowCompletionScreen;
        }
    }

    private void OnDestroy()
    {
        if (_target != null)
        {
            _target._onTaskSuccessChanged -= UpdateDescription;
            _target._onCompleted -= ShowCompletionScreen;
        }
    }

    private void UpdateDescription(Quest achievement, Task task, int currentSuccess, int prevSuccess)
        => _description.text = BuildTaskDescription(task);

    private void ShowCompletionScreen(Quest achievement)
        => _completionScreen.SetActive(true);
    private string BuildTaskDescription(Task task) => $"{task.Description} {task.CurrentSuccess}/{task.NeedSuccessToComplete}";
}
