using UnityEditor;
using UnityEngine;

public abstract class InitialSuccessValue : ScriptableObject
{
    public abstract int GetValue(QuestTask task);
}
