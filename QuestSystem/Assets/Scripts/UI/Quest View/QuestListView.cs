using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class QuestListView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _elementTextprefab;

    private Dictionary<Quest, GameObject> _elementsByQuest = new Dictionary<Quest, GameObject>();

    private ToggleGroup _toggleGroup;

    private void Awake()
    {
        _toggleGroup = GetComponent<ToggleGroup>();
    }

    public void AddElement( Quest quest, UnityAction<bool> onClicked)
    {
        var element = Instantiate(_elementTextprefab, transform);

        element.text = quest.DisplayName;

        var toggle = element.GetComponent<Toggle>();
        toggle.group = _toggleGroup;
        toggle.onValueChanged.AddListener(onClicked);

        _elementsByQuest.Add(quest, element.gameObject);
    }
    public void RemoveElement(Quest quest)
    {
        Destroy(_elementsByQuest[quest]);
        _elementsByQuest.Remove(quest);
    }
}
