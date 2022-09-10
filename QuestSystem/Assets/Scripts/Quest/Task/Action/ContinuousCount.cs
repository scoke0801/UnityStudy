using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Quest/Task/Action/ContinuousCount", fileName = "ContinuousCount")]
public class ContinuousCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return successCount > 0 ? currentSuccess + successCount : 0;
    }
}

