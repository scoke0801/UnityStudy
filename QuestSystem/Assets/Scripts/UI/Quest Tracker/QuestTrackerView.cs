using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;



public class QuestTrackerView : MonoBehaviour
{
    [SerializeField]
    QuestTracker _questTrackerPrefab;

    [SerializeField]
    private CategoryColor[] _categoryColors;


    private void Start()
    {
        QuestSystem.Instance._onQuestRegistered += CreateQuestTracker;

        foreach( var quest in QuestSystem.Instance.ActiveQuests)
        {
            CreateQuestTracker(quest);
        }
    }

    private void OnDestroy()
    {
        if(QuestSystem.Instance)
        {
            QuestSystem.Instance._onQuestRegistered -= CreateQuestTracker;
        }
    }
    private void CreateQuestTracker(Quest quest)
    {
        var categoryColor = _categoryColors.FirstOrDefault(x => x.category == quest.Category);
        var color = categoryColor.category == null ? Color.white : categoryColor.color;

        Instantiate(_questTrackerPrefab, transform).Setup(quest, color);
    }


    [System.Serializable]
    private struct CategoryColor
    {
        public Category category;
        public Color color;
    }

}
