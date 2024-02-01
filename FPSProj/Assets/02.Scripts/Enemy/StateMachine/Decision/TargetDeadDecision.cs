using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 타겟이 죽었는 지 체크...
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
                Debug.LogError("생명력 관리 컴포넌트 HealthBase 없음!" + controller.name, controller.gameObject);
            }
            return false;
        }
    }
}