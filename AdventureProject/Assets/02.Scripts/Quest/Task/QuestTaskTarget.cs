using UnityEditor;
using UnityEngine;

public abstract class QuestTaskTarget : ScriptableObject
{
    public abstract object Value { get; }
    public abstract bool IsEqual(object target);
}
