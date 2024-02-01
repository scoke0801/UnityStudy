 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// navMeshAgent���� ���� �Ÿ��� ���ߴ� ���� ������ �� ���� �ʾҰų�,
    /// ��θ� �˻����� �ƴ϶�� true
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