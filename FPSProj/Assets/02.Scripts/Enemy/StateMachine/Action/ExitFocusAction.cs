using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 타겟을 놓치고 다 초기화 하는 앤셕
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/Exit Focus")]
    public class ExitFocusAction : Action
    { 
        public override void OnReadyAction(StateController controller)
        {
            // 초기화. 
            controller.focusSight = false;
            controller.variables.feelAlert = false;
            controller.variables.heartAlert = false;
            controller.Strafing = false;
            controller.navAgent.destination = controller.personalTarget;
            controller.navAgent.speed = 0f;
        }
        public override void Act(StateController controller)
        { 
        }
          
    }

}