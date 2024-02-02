using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 공격과 동시에 이동하는 액션.
    /// 일단 회전할 때는 회전을 하고, 회전을 다 했으면
    /// strafing이 활성화.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/FocusMove")]
    public class FocusMoveAction : Action
    {
        public ClearShotDecision clearShotDecision;

        private Vector3 currentDest; // 현재 이동 방향
        private bool aligned;//     타겟이 같은 방향에 있는 지.


        public override void OnReadyAction(StateController controller)
        {
            // 초기화. 
            controller.hadClearShot = controller.haveClearShot = false;

            currentDest = controller.navAgent.destination;

            controller.focusSight = false;
            aligned = false;
        }

        public override void Act(StateController controller)
        {
            if (!aligned)
            {
                controller.navAgent.destination = controller.personalTarget;
                controller.navAgent.speed = 0f;

                if(controller.enemyAnimation.angularSpeed == 0f)
                {
                    // 각도가 0.
                    // -> 플레이어를 바라보고 있을 때.
                    controller.Strafing = true;
                    aligned = true;
                    controller.navAgent.destination = currentDest;
                    controller.navAgent.speed = controller.generalStats.evadeSpeed;
                }
                else
                {
                    controller.haveClearShot = clearShotDecision.Decide(controller);
                    if(controller.hadClearShot != controller.haveClearShot)
                    {
                        controller.Aiming = controller.haveClearShot;
                        // 사격이 가능하다면, 현재 이동 목표가 엄폐물과 다르더라도 일단 이동하지 말자.
                        if(controller.haveClearShot && !Equals(currentDest, controller.CoverSpot))
                        {
                            controller.navAgent.destination = controller.transform.position;
                        }
                    }

                    controller.hadClearShot = controller.haveClearShot;
                }
            }
        }
    }

}