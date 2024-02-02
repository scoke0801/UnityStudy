using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// ���ݰ� ���ÿ� �̵��ϴ� �׼�.
    /// �ϴ� ȸ���� ���� ȸ���� �ϰ�, ȸ���� �� ������
    /// strafing�� Ȱ��ȭ.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/FocusMove")]
    public class FocusMoveAction : Action
    {
        public ClearShotDecision clearShotDecision;

        private Vector3 currentDest; // ���� �̵� ����
        private bool aligned;//     Ÿ���� ���� ���⿡ �ִ� ��.


        public override void OnReadyAction(StateController controller)
        {
            // �ʱ�ȭ. 
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
                    // ������ 0.
                    // -> �÷��̾ �ٶ󺸰� ���� ��.
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
                        // ����� �����ϴٸ�, ���� �̵� ��ǥ�� ���󹰰� �ٸ����� �ϴ� �̵����� ����.
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