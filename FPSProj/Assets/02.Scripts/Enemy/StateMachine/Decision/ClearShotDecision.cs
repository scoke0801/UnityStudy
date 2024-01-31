using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 1. ���� üũ�� �ϴµ�, ��ó�� ��ֹ��̳� ������ ������ �ִ� �� üũ.
    /// 2. Ÿ�� ��ǥ���� ��ֹ��̳� ������ �ִ� �� üũ.
    /// ����, ó�� ����� �浹ü�� �÷��̾��� ���� �� ���ٴ� ��.
    /// ->
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Clear Shot")]
    public class ClearShotDecision : Decision
    {
        [Header("Extra Decision")]
        public FocusDecision targetNear;

        private bool HaveClearShot(StateController controller)
        {
            Vector3 shotOrigin = controller.transform.position +
                Vector3.up * (controller.generalStats.aboveCoverHeight + controller.navAgent.radius);

            Vector3 shotDireciton = controller.personalTarget - shotOrigin;

            // ù��° Ȯ��. ��ó�� ��ֹ��̳� ������ ������ �ִ� �� üũ.
            bool blockedShot = Physics.SphereCast(shotOrigin, controller.navAgent.radius,
                shotDireciton, out RaycastHit hit, controller.nearRadius,
                controller.generalStats.coverMask | controller.generalStats.obstacleMask);

            if (!blockedShot)
            {
                // 2��° Ȯ��. Ÿ�� ��ǥ���� ��ֹ��̳� ������ �ִ� �� üũ
                blockedShot = Physics.Raycast(shotOrigin, shotDireciton, out hit, shotDireciton.magnitude,
                    controller.generalStats.coverMask | controller.generalStats.obstacleMask);

                if (blockedShot)
                {
                    blockedShot = !(hit.transform.root == controller.aimTarget.root);   
                }
            }
            return !blockedShot;
        }

        public override bool Decide(StateController controller)
        {
            return targetNear.Decide(controller) || HaveClearShot(controller);
        }
    }

}