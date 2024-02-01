using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 타겟이 있다면, 타겟까지 이동하지만 타겟을 잃었다면 가만히 서 있습니다.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/Search")]
    public class SearchAction : Action
    {
        // 초기화
        public override void OnReadyAction(StateController controller)
        {
            controller.focusSight = false;
            controller.enemyAnimation.AbortPendingAim();
            controller.enemyAnimation.anim.SetBool(Defs.AnimatorKey.Crouch, false);
            controller.CoverSpot = Vector3.positiveInfinity;
        }

        public override void Act(StateController controller)
        {
            // 타겟이 없는 경우.
            if(Equals(controller.personalTarget, Vector3.positiveInfinity))
            {
                controller.navAgent.destination = controller.transform.position;
            }
            else
            {
                // 타겟이 존재하는 경우.
                controller.navAgent.speed = controller.generalStats.chaseSpeed;
                controller.navAgent.destination = controller.personalTarget;
            }
        }
    }
}