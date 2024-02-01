 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// navMeshAgent에서 남은 거리가 멈추는 중일 정도로 얼마 남지 않았거나,
    /// 경로를 검색중이 아니라면 true
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Reached Point")]
    public class ReachedPointDecision : Decision
    {

        public override bool Decide(StateController controller)
        {
            if (Application.isPlaying == false)
            {
                return false;
            }

            if(controller.navAgent.remainingDistance <= controller.navAgent.stoppingDistance && 
                !controller.navAgent.pathPending)
            {
                return true;
            }
            return false;
        }

    }

}