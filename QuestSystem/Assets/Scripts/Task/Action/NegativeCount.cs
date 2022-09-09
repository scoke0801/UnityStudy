using UnityEngine;
using UnityEditor; 

[CreateAssetMenu(menuName = "Quest/Task/Action/NegativeCount", fileName = "NagativeCount")]
public class NegativeCount : TaskAction
{
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return successCount < 0 ? successCount + currentSuccess : currentSuccess;
    }
}
