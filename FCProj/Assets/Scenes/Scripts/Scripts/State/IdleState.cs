using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace State
{
    public class IdleState : State<EnemyContorller>
    {
        private Animator _animator;
        private CharacterController _controller;

        protected int _hasMove = Animator.StringToHash("Move");
        protected int _hasMoveSpeed = Animator.StringToHash("MoveSpeed");


        public override void OnInitialized()
        {
            _animator = context.GetComponent<Animator>();
            _controller = context.GetComponent<CharacterController>();
        }

        public override void OnEnter()
        {
            _animator?.SetBool(_hasMove, false);
            _animator?.SetFloat(_hasMoveSpeed, 0);

            _controller?.Move(Vector3.zero);
        }

        public override void Update(float deltaTime)
        {
            Transform enemy = context.SearchEnemy();

            if(enemy != null)
            {
                if (context.IsAvailableAttack)
                {
                    stateMachine.ChangeState<AttackState>();
                }
                else
                {
                    stateMachine.ChangeState<MoveState>();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

    }
}