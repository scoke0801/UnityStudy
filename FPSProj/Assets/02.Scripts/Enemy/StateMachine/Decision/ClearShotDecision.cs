using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 1. 더블 체크를 하는데, 근처에 장애물이나 엄폐물이 가깝게 있는 지 체크.
    /// 2. 타켓 목표까지 장애물이나 엄폐물이 있는 지 체크.
    /// 만약, 처음 검출된 충돌체가 플레이어라면 막힌 게 없다는 뜻.
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

            // 첫번째 확인. 근처에 장애물이나 엄폐물이 가깝게 있는 지 체크.
            bool blockedShot = Physics.SphereCast(shotOrigin, controller.navAgent.radius,
                shotDireciton, out RaycastHit hit, controller.nearRadius,
                controller.generalStats.coverMask | controller.generalStats.obstacleMask);

            if (!blockedShot)
            {
                // 2번째 확인. 타켓 목표까지 장애물이나 엄폐물이 있는 지 체크
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