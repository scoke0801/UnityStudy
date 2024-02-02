using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 그냥 멈춰서 지점을 바라본다...
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/SpotFocus")]
    public class SpotFocusAction : Action
    { 
        public override void Act(StateController controller)
        {
            controller.navAgent.destination = controller.personalTarget;
            controller.navAgent.speed = 0f;
        }
    }

}