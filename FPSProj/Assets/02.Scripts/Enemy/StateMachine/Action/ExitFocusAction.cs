using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Ÿ���� ��ġ�� �� �ʱ�ȭ �ϴ� �ؼ�
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/Exit Focus")]
    public class ExitFocusAction : Action
    { 
        public override void OnReadyAction(StateController controller)
        {
            // �ʱ�ȭ. 
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