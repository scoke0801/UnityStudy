using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Quest/Task/Action/PositiveCount", fileName = "PositiveCount")]
public class PositiveCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return successCount > 0 ? successCount + currentSuccess : currentSuccess;
    }
}