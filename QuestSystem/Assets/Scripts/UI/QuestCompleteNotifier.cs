using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using TMPro;

public class QuestCompleteNotifier : MonoBehaviour
{
    [SerializeField]
    private string _titleDesciptor;


    [SerializeField]
    private TextMeshProUGUI _titleText;


    [SerializeField]
    private TextMeshProUGUI _rewardText;

    [SerializeField]
    private float _showTime = 3.0f;

    private Queue<Quest> _reservedQuests = new Queue<Quest>();

    private StringBuilder _stringBuilder = new StringBuilder();

    private void Start()
    {
        var questSystem = QuestSystem.Instance;
        questSystem._onQuestCompleted += Notify;
        questSystem._onAchievementCompleted += Notify;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        var questSystem = QuestSystem.Instance;

        if (questSystem != null)
        {
            questSystem._onQuestCompleted -= Notify;
            questSystem._onAchievementCompleted += Notify;
        }
    }
    private void Notify(Quest quest)
    {
        _reservedQuests.Enqueue(quest);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            StartCoroutine(ShowNotice());
        }
    }
    private IEnumerator ShowNotice()
    {
        var waitSeconds = new WaitForSeconds(_showTime);

        Quest quest;
        while( _reservedQuests.TryDequeue(out quest))
        {
            _titleText.text = _titleDesciptor.Replace("%{dn}", quest.DisplayName);
            foreach(var reward in quest.Rewards)
            {
                _stringBuilder.Append(reward.Description);
                _stringBuilder.Append(" ");
                _stringBuilder.Append(reward.Quantity);
                _stringBuilder.Append(" ");
            }
            _rewardText.text = _stringBuilder.ToString();

            _stringBuilder.Clear();

            yield return waitSeconds;
        }

        gameObject.SetActive(false); 
    } 
}
