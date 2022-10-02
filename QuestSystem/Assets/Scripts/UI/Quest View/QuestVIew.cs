using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestVIew : MonoBehaviour
{
    [SerializeField]
    private QuestListViewController _questListViewController;

    [SerializeField]
    private QuestDetailVIew _questDetailView;

    private void Start()
    {
        var questSystem = QuestSystem.Instance;

        foreach (var quest in questSystem.ActiveQuests)
        {
            AddQuestToActiveListView(quest);
        }

        foreach (var quest in questSystem.CompletedQuests)
        {
            AddQuestToCompletedListView(quest);
        }

        questSystem._onQuestRegistered += AddQuestToActiveListView;
        
        questSystem._onQuestCompleted += RemoveQuestFromActiveListView;
        questSystem._onQuestCompleted += AddQuestToCompletedListView;
        questSystem._onQuestCompleted += HideDetailIfQuestCanceled;

        questSystem._onQuestCanceled += HideDetailIfQuestCanceled;
        questSystem._onQuestCanceled += RemoveQuestFromActiveListView;
    
        foreach(var tab in _questListViewController.Tabs)
        {
            tab.onValueChanged.AddListener(HideDetail);
        }

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        var questSystem = QuestSystem.Instance;
        if (questSystem)
        {
            questSystem._onQuestRegistered -= AddQuestToActiveListView;

            questSystem._onQuestCompleted -= RemoveQuestFromActiveListView;
            questSystem._onQuestCompleted -= AddQuestToCompletedListView;
            questSystem._onQuestCompleted -= HideDetailIfQuestCanceled;

            questSystem._onQuestCanceled -= HideDetailIfQuestCanceled;
            questSystem._onQuestCanceled -= RemoveQuestFromActiveListView;
        }
    }

    private void OnEnable()
    {
        if(_questDetailView.Target!=null)
        {
            _questDetailView.Show(_questDetailView.Target);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }

    private void ShowDetail(bool isOn, Quest quest)
    {
        if (isOn)
        {
            _questDetailView.Show(quest);
        }
    }

    private void HideDetail(bool isOn)
    {
        _questDetailView.Hide();
    }

    private void AddQuestToActiveListView(Quest quest)
        => _questListViewController.AddQuestToActiveListView(quest, ison => ShowDetail(ison, quest));

    private void AddQuestToCompletedListView(Quest quest)
        => _questListViewController.AdQuestToCompletedListView(quest, ison => ShowDetail(ison, quest));

    private void HideDetailIfQuestCanceled(Quest quest)
    {
        if(_questDetailView.Target == quest)
        {
            _questDetailView.Hide();
        }
    }

    private void RemoveQuestFromActiveListView(Quest quest)
    {
        _questListViewController.RemoveQuestFromActiveListView(quest);
        if(_questDetailView.Target == quest)
        {
            _questDetailView.Hide();
        }
    }
}
