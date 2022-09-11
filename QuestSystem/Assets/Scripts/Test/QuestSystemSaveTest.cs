using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystemSaveTest : MonoBehaviour
{
    [SerializeField]
    private Quest quest;

    [SerializeField]
    private Category category;

    [SerializeField]
    private TaskTarget target;

    void Start()
    {
        QuestSystem questSystem = QuestSystem.Instance;

        if(questSystem.ActiveQuests.Count == 0)
        {
            Debug.Log("Register");
            var newQuest = questSystem.Register(quest);
        }
        else
        {
            questSystem._onQuestCompleted += (quest) =>
            {
                Debug.Log("Completed");
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            };
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            QuestSystem.Instance.ReceiveReport(category, target, 1);
        }
    }
}
