using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 타겟이 시야가 막히지 안은 상태에서 타겟이 시야각(1/2)사이에 있다면
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Lock")]
    public class LookDecision : Decision
    {
        private bool MyHandleTargets(StateController controller, bool hasTarget, Collider[] targetsInRadius)
        {
            if (hasTarget)
            {
                // 타겟(플레이어)의 위치.
                Vector3 target = targetsInRadius[0].transform.position;
                Vector3 dirToTarget = target - controller.transform.position;

                bool inFOVCondition = (Vector3.Angle(controller.transform.forward, dirToTarget) <
                    controller.viewAngle * 0.5f);

                // 시야각 안에 존재하고, 시야가 막히지 않았다면.
                if(inFOVCondition && !controller.BlockedSight())
                {
                    controller.targetInSight = true;
                    controller.personalTarget = controller.aimTarget.position;

                    return true;
                }
            }
            return false;
        }

        public override bool Decide(StateController controller)
        {
            controller.targetInSight = false;

            return CheckTargetsInradius(controller, controller.viewRadius, MyHandleTargets);
        }
    }

}