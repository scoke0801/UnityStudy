using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// alertCheck�� ���� ��� ����ų�( �ѼҸ��� ��Ȱų� )
    /// Ư���Ÿ����� �þ߰� ���� �־, Ư�� ��ġ���� Ÿ���� ��ġ�� ������ ���� �Ǿ��� ���.
    /// ����ٶ�� �Ǵ�.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Hear")]
    public class HearDecision : Decision
    {
        private Vector3 lastPos, currentPos;

        public override void OnEnableDecision(StateController controller)
        {
            // �ʱ�ȭ.
            lastPos = currentPos = Vector3.positiveInfinity;
        }

        private bool MyHandleTargets(StateController controller, bool hasTarget, Collider[] targetInHearRadius)
        {
            // Ÿ���� �����ϴ� ���.
            if (hasTarget)
            {
                currentPos = targetInHearRadius[0].transform.position;

                // ������ Ž���� ��ġ�� �����ϴ� ���.
                if(!Equals(lastPos, Vector3.positiveInfinity))
                {
                    // 2�� Ž���� ������ ���.
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