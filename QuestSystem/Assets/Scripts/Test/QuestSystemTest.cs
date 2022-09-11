using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystemTest : MonoBehaviour
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

        questSystem._onQuestRegistered += (quest) =>
        {
            print($"New Quest:{quest.CodeName} Registered");
            print($"Active Quests Count : {questSystem.ActiveQuests.Count}");
        };

        questSystem._onQuestCompleted += (quest) =>
        {
            print($"Quest:{quest.CodeName} Completed");
            print($"Completed Quests Count : {questSystem.CompletedQuests.Count}");
        };

        Quest newQuest = questSystem.Register(quest);
        newQuest._onTaskSuccessChanged += (quest, task, currentSucces, prevSuccess) =>
        {
            print($"Quest:{quest.CodeName}, Task:{task.CodeName}, CurrentSuccess:{currentSucces}");
        };
    }
     
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            QuestSystem.Instance.ReceiveReport(category, target, 1);
        }
    }
}
