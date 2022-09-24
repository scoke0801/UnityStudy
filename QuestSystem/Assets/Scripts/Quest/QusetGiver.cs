using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QusetGiver : MonoBehaviour
{
    [SerializeField]
    private Quest[] _quests;

    private void Start()
    {
        foreach(var quest in _quests)
        {
            if (quest.IsAcceptable && QuestSystem.Instance.ContainsInCompltedQuest(quest))
            {
                QuestSystem.Instance.Register(quest);
            }
        }
    }


}
