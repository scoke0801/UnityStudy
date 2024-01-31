using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{

    /// <summary>
    /// Sense 타입에 따라 특정 거리로 부터 가깝진 않지만 시야는 막히지 않았지만 위험요소를 감지했거나
    /// 너무 가까운 거리에 타겟(플레이어)이 있는 지 판단.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Focus")]
    public class FocusDecision : Decision
    {
        public enum Sense
        {
            NEAR,           // 가까운가
            PERCEPTION,     // 인지 범위에 있는가
            VIEW,           // 보이는가
        }

        [Tooltip("어떤 크기로 위험 요소 감지를 할지 결정할 타입.")]
        public Sense sense;

        [Tooltip("현재 엄폐물을 해제할 지 여부")]
        public bool invalidateCoverSpot;

        private float radius;   // sense->range

        public override void OnEnableDecision(StateController controller)
        {
            switch (sense)
            {
                case Sense.NEAR:
                    radius = controller.nearRadius;
                    break;
                case Sense.PERCEPTION:
                    radius = controller.perceptionRadius;
                    break;
                case Sense.VIEW:
                    radius = controller.viewRadius;
                    break;

                default:
                    radius = controller.nearRadius;
                    break;
            }
        }

        private bool MyHandleTargets(StateController controller, bool hasTarget, Collider[] targetsInNearRadius)
        {
            // 타겟(플레이어)이 존재하고 시야가 막히지 않았다면
            if (hasTarget && !controller.BlockedSight())
            {
                // 
                if (invalidateCoverSpot)
                {
                    controller.CoverSpot = Vector3.positiveInfinity;
                }

                controller.targetInSight = true;
                controller.personalTarget = controller.aimTarget.position;

                return true;
            }

            return false;
        }

        public override bool Decide(StateController controller)
        {
            // 1.가까운 거리의 인지범위가 아닌데, 주변에서 경고를 받고 시야가 막히지 않았다면.
            // 2. 타겟이 거리안에 들어왔다면
            return (sense != Sense.NEAR && controller.variables.feelAlert && !controller.BlockedSight()) ||
                Decision.CheckTargetsInradius(controller, radius, MyHandleTargets);
        }
    }
}