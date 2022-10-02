using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class QuestListViewController : MonoBehaviour
{
    [SerializeField]
    private ToggleGroup _tabGroup;

    [SerializeField]
    private QuestListView _activeQuestListView;

    [SerializeField]
    private QuestListView _completedQuestListView;

    public IEnumerable<Toggle> Tabs => _tabGroup.ActiveToggles();

    public void AddQuestToActiveListView(Quest quest, UnityAction<bool> onClicked)
        => _activeQuestListView.AddElement(quest, onClicked);

    public void RemoveQuestFromActiveListView(Quest quest)
        => _activeQuestListView.RemoveElement(quest);

    public void AdQuestToCompletedListView(Quest quest, UnityAction<bool> onClicked)
    => _completedQuestListView.AddElement(quest, onClicked);
}
