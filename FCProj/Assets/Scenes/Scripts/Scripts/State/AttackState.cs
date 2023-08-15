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
            // �̹� ���� ������ �����̱⿡ �ش� ���·� �����߰�����, �ٽ� �ѹ� �˻�.
            // �����൵ ������ ��.
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