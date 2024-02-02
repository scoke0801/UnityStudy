using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/ReturnToCover")]
    public class ReturnToCoverAction : Action
    {
        public override void OnReadyAction(StateController controller)
        {
            if (!Equals(controller.CoverSpot, Vector3.positiveInfinity))
            {
                controller.navAgent.destination = controller.CoverSpot;
                controller.navAgent.speed = controller.generalStats.chaseSpeed;
                if (Vector3.Distance(controller.CoverSpot, controller.transform.position) > 0.5f)
                {
                    controller.enemyAnimation.AbortPendingAim();
                }
            }
            else
            {
                // 엄폐물이 없는경우..
                controller.navAgent.destination = controller.transform.position;
            }
        }
        public override void Act(StateController controller)
        {
            if (!Equals(controller.CoverSpot, controller.transform.position))
            {
                // 엄폐물과 위치가 다르면 타케팅 취소.
                controller.focusSight = false;
            } 
        }
    }

}