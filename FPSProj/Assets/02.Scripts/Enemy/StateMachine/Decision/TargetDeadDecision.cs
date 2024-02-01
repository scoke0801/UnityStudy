using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Ÿ���� �׾��� �� üũ...
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Target Dead")]
    public class TargetDeadDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            try
            {
                return controller.aimTarget.root.GetComponent<HealthBase>().IsDead;
            }
            catch (UnassignedReferenceException)
            {
                Debug.LogError("����� ���� ������Ʈ HealthBase ����!" + controller.name, controller.gameObject);
            }
            return false;
        }
    }
}