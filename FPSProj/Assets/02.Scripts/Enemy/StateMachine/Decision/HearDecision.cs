using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// alertCheck를 통해 경고를 들었거나( 총소리가 들렸거나 )
    /// 특정거리에서 시야가 막혀 있어도, 특정 위치에서 타겟의 위치가 여러번 인지 되었을 경우.
    /// 들었다라고 판단.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Hear")]
    public class HearDecision : Decision
    {
        private Vector3 lastPos, currentPos;

        public override void OnEnableDecision(StateController controller)
        {
            // 초기화.
            lastPos = currentPos = Vector3.positiveInfinity;
        }

        private bool MyHandleTargets(StateController controller, bool hasTarget, Collider[] targetInHearRadius)
        {
            // 타겟이 존재하는 경우.
            if (hasTarget)
            {
                currentPos = targetInHearRadius[0].transform.position;

                // 이전에 탐색한 위치가 존재하는 경우.
                if(!Equals(lastPos, Vector3.positiveInfinity))
                {
                    // 2번 탐색에 성공한 경우.
                    if(!Equals(lastPos, currentPos))
                    {
                        controller.personalTarget = currentPos;
                        return true;
                    }
                }
                lastPos = currentPos;
            }

            return false; 
        }

        public override bool Decide(StateController controller)
        {
            if (controller.variables.heartAlert)
            {
                controller.variables.heartAlert = false;
                return true;
            }
            return CheckTargetsInradius(controller, controller.perceptionRadius, MyHandleTargets);
        }
    }
}