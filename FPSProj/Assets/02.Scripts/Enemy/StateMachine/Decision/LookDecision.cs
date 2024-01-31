using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Ÿ���� �þ߰� ������ ���� ���¿��� Ÿ���� �þ߰�(1/2)���̿� �ִٸ�
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Lock")]
    public class LookDecision : Decision
    {
        private bool MyHandleTargets(StateController controller, bool hasTarget, Collider[] targetsInRadius)
        {
            if (hasTarget)
            {
                // Ÿ��(�÷��̾�)�� ��ġ.
                Vector3 target = targetsInRadius[0].transform.position;
                Vector3 dirToTarget = target - controller.transform.position;

                bool inFOVCondition = (Vector3.Angle(controller.transform.forward, dirToTarget) <
                    controller.viewAngle * 0.5f);

                // �þ߰� �ȿ� �����ϰ�, �þ߰� ������ �ʾҴٸ�.
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