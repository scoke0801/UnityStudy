using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public class AttackState_New : State<EnemyController>
    {
        private Animator _animator;
        private AttackStateController _attackStateController;
        private IAttackable _attackable;

        protected int _attackTriggerHash = Animator.StringToHash("AttackTrigger");
        protected int _attackIndexHash = Animator.StringToHash("AttackIndex");

        public override void OnInitialized() 
        {
            _animator = context.GetComponent<Animator>();

            _attackStateController = context.GetComponent<AttackStateController>();

            _attackable = context.GetComponent<IAttackable>();

        }

        public override void OnEnter() 
        {
            // 공격 상태가 아닌 경우.
            if(_attackable == null || _attackable.CurrentAttackBehaviour == null)
            {
                stateMachine.ChangeState<IdleState>();
                return;
            }

            _attackStateController._enterAttackStateHandelr += OnEnterAttackState;
            _attackStateController._exitAttackStateHandler += OnExitAttackState;

            _animator?.SetInteger(_attackIndexHash, _attackable.CurrentAttackBehaviour._animationIndex);
            _animator?.SetTrigger(_attackTriggerHash);
        }

        public override void OnExit()
        {
            _attackStateController._enterAttackStateHandelr -= OnEnterAttackState;
            _attackStateController._exitAttackStateHandler -= OnExitAttackState;
        }
        public void OnEnterAttackState()
        {

        }

        public void OnExitAttackState()
        {

        }
    }
}