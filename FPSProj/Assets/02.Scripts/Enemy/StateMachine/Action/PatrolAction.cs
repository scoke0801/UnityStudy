using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(menuName = "PluggableAI/Actions/Patrol")]
    public class PatrolAction : Action
    {
        public override void OnReadyAction(StateController controller)
        {
            // √ ±‚»≠.
            controller.enemyAnimation.AbortPendingAim();
            controller.enemyAnimation.anim.SetBool(Defs.AnimatorKey.Crouch, false);
            controller.personalTarget = Vector3.positiveInfinity;
            controller.CoverSpot = Vector3.positiveInfinity;
        }
        public void Patrol(StateController controller)
        {
            if(controller.patrolWayPoints.Count == 0)
            {
                return;
            }

            controller.focusSight = false;
            controller.navAgent.speed = controller.generalStats.patrolSpeed;
            if(controller.navAgent.remainingDistance <= controller.navAgent.stoppingDistance && !controller.navAgent.pathPending)
            {
                controller.variables.patrolTimer += Time.deltaTime;
                if (controller.variables.patrolTimer >= controller.generalStats.patrolWaitTime)
                {
                    controller.wayPointIndex = (controller.wayPointIndex + 1) % controller.patrolWayPoints.Count;
                    controller.variables.patrolTimer = 0;
                }
            }

            try
            {
                controller.navAgent.destination = controller.patrolWayPoints[controller.wayPointIndex].position;
            }
            catch (UnassignedReferenceException)
            {
                Debug.LogWarning("No Patrol WayPoints....", controller.gameObject);
                controller.patrolWayPoints = new List<Transform>
                {
                    controller.transform
                };

                controller.navAgent.destination = controller.transform.position;

            }
        }
        public override void Act(StateController controller)
        {
            Patrol(controller);
        }
    }
}