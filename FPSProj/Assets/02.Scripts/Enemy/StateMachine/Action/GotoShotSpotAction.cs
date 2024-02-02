using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Ÿ���� ���̴µ�, ��ȿ��ݰŸ��� �����ʴٸ� �ش� �������� �̵��ؾ���.
    /// �̵��� ���� �׼��� �� ��ũ��Ʈ.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/GotoShot Spot")]
    public class GotoShotSpotAction : Action
    {
        public override void OnReadyAction(StateController controller)
        {
            // �ʱ�ȭ.
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