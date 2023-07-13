using UnityEditor;
using UnityEngine;

public abstract class QuestTaskAction : ScriptableObject
{
    public abstract int Run(QuestTask task, int currentSuccess, int successCount);
}
