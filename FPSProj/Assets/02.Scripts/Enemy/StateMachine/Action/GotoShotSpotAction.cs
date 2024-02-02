using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 타겟이 보이는데, 유효사격거리에 있지않다면 해당 지점까지 이동해야함.
    /// 이동할 때의 액션이 이 스크립트.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/GotoShot Spot")]
    public class GotoShotSpotAction : Action
    {
        public override void OnReadyAction(StateController controller)
        {
            // 초기화.
            controller.focusSight = false;
            controller.navAgent.destination = controller.personalTarget;
            controller.navAgent.speed = controller.generalStats.chaseSpeed;
            controller.enemyAnimation.AbortPendingAim();
        }
        public override void Act(StateController controller)
        {

        }
    }

}