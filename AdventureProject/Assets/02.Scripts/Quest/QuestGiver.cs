using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] private Quest[] _quests;

    private void Start()
    {
        for(int i = 0; i < _quests.Length; ++i)
        {
            if (_quests[i].IsAcceptable && !QuestSystem.Instance.ContainsInCompltedQuest(_quests[i]))
            {
                QuestSystem.Instance.Register(_quests[i]);
            }
        }
    }
}