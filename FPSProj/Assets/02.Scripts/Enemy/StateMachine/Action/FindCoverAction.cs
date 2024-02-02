using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 숨을 수 있는 엄폐물이 없다면 가만히 서있는다.
    /// 만약 새로운 엄페물이 있고 엄폐물보다 가깝다면, 엄폐물을 변경.
    /// + 숨어서 총알 장전.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/FindCover")]
    public class FindCoverAction : Action
    {
        public override void OnReadyAction(StateController controller)
        {
            // 초기화.
            controller.focusSight = false;
            controller.enemyAnimation.AbortPendingAim();
            controller.enemyAnimation.anim.SetBool(Defs.AnimatorKey.Crouch, false);

            ArrayList nextCoverData = controller.coverLookup.GetBestCoverSpot(controller);
            Vector3 potentialCover = (Vector3)nextCoverData[1];

            if (Vector3.Equals(potentialCover, Vector3.positiveInfinity))
            {
                controller.navAgent.destination = controller.transform.position;
                return;
            }
            else if ((controller.personalTarget - potentialCover).sqrMagnitude <
                (controller.personalTarget - controller.CoverSpot).sqrMagnitude &&
                !controller.IsNearOtherSpot(potentialCover, controller.nearRadius))
            {
                // 새로 찾은 엄폐물까지의 거리가 기존 엄폐물보다 가까이에 있고
                // 다른 가까운 엄폐물이 없다면.
                controller.coverHash = (int)nextCoverData[0];
                controller.CoverSpot = potentialCover;
            }
            controller.navAgent.destination = controller.CoverSpot;
            controller.navAgent.speed = controller.generalStats.evadeSpeed;

            controller.variables.currentShoots = controller.variables.shotsinRounds;
        }

        public override void Act(StateController controller)
        {
        }
    }

}