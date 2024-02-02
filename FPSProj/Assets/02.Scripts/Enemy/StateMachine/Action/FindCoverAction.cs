using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// ���� �� �ִ� ������ ���ٸ� ������ ���ִ´�.
    /// ���� ���ο� ���买�� �ְ� ���󹰺��� �����ٸ�, ������ ����.
    /// + ��� �Ѿ� ����.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/FindCover")]
    public class FindCoverAction : Action
    {
        public override void OnReadyAction(StateController controller)
        {
            // �ʱ�ȭ.
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
                // ���� ã�� ���󹰱����� �Ÿ��� ���� ���󹰺��� �����̿� �ְ�
                // �ٸ� ����� ������ ���ٸ�.
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