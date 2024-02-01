using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Ÿ���� �ִٸ�, Ÿ�ٱ��� �̵������� Ÿ���� �Ҿ��ٸ� ������ �� �ֽ��ϴ�.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/Search")]
    public class SearchAction : Action
    {
        // �ʱ�ȭ
        public override void OnReadyAction(StateController controller)
        {
            controller.focusSight = false;
            controller.enemyAnimation.AbortPendingAim();
            controller.enemyAnimation.anim.SetBool(Defs.AnimatorKey.Crouch, false);
            controller.CoverSpot = Vector3.positiveInfinity;
        }

        public override void Act(StateController controller)
        {
            // Ÿ���� ���� ���.
            if(Equals(controller.personalTarget, Vector3.positiveInfinity))
            {
                controller.navAgent.destination = controller.transform.position;
            }
            else
            {
                // Ÿ���� �����ϴ� ���.
                controller.navAgent.speed = controller.generalStats.chaseSpeed;
                controller.navAgent.destination = controller.personalTarget;
            }
        }
    }
}