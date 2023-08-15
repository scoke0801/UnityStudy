using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public class EndOfAttackStateMachineBehaviour : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<EnemyController>()?.StateMachine.ChangeState<IdleState>();
        }
    }
}
