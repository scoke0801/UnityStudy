using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/TakeCover")]
    public class TakeCoverAction : Action
    {
        private readonly int coverMin = 2;  // 장애물에 머물러 있을 최소 시간
        private readonly int coverMax = 5;  // 장애물에 머물러 있을 최대 시간

        public override void OnReadyAction(StateController controller)
        {
            // 초기화.
            controller.variables.feelAlert = false;
            controller.variables.watiInCoverTime = 0f;
            if(!Equals(controller.CoverSpot, Vector3.positiveInfinity))
            {
                controller.enemyAnimation.anim.SetBool(Defs.AnimatorKey.Crouch, false);
                controller.variables.coverTime = Random.Range(coverMin, coverMax);
            }
        }
        public override void Act(StateController controller)
        {
            if (!controller.reloading)
            {
                controller.variables.watiInCoverTime += Time.deltaTime;
            }

            controller.variables.blindEngageTimer += Time.deltaTime;
            if (controller.enemyAnimation.anim.GetBool(Defs.AnimatorKey.Crouch))
            {
                Rotating(controller);
            }
        }

        public void Rotating(StateController controller)
        {
            Vector3 dirToVector = controller.personalTarget - controller.transform.position;
            if (dirToVector.sqrMagnitude < 0.0001f || dirToVector.sqrMagnitude > 100000000f) 
            {
                // 값이 너무 작거나 큰 경우 무시.
                return;
            }
            Quaternion targetRotation = Quaternion.LookRotation(dirToVector);
            if(Quaternion.Angle(controller.transform.rotation, targetRotation) > 5f)
            {
                controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, targetRotation, 10f * Time.deltaTime);
            }
        }
    }

}