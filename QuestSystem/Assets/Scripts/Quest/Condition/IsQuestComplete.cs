using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Condition/IsQuestComplete", fileName = "IsQuestComplete_")]
public class IsQuestComplete : Condition
{
    [SerializeField]
    private Quest _target;

    public override bool IsPass(Quest quest)
        => QuestSystem.Instance.ContainsInCompltedQuest(_target);
}
