using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public class AttackState : State<EnemyContorller>
    {
        private Animator _animator;
        private int _hashAttack = Animator.StringToHash("Attack");

        public override void OnInitialized() 
        {
            _animator = context.GetComponent<Animator>();
        }

        public override void OnEnter() 
        {
            // 이미 공격 가능한 상태이기에 해당 상태로 진입했겠지만, 다시 한번 검사.
            // 안해줘도 무방할 듯.
            if (context.IsAvailableAttack)
            {
                _animator?.SetTrigger(_hashAttack);
            }
            else
            {
                stateMachine.ChangeState<IdleState>();
            }
        }
    }
}