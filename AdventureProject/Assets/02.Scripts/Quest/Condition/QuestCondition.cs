using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Condition", fileName = "Condition_")]
public abstract class QuestCondition : ScriptableObject
{
    [SerializeField]
    private string _description;

    public string Description => _description;

    public abstract bool IsPass(Quest quest);
}
