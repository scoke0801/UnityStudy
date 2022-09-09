using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName ="Quest/Task/Action/SetCount", fileName = "SetCount")]
public class SetCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return successCount;
    }
} 