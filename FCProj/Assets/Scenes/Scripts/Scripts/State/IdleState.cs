using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace State
{
    public class IdleState : State<EnemyController>
    {
        private bool _isPatrol = false;
        private float _minIdleTime = 0.0f;
        private float _maxIdleTime = 3.0f;
        private float _idleTime = 0.0f;

        private Animator _animator;
        private CharacterController _controller;

        protected int _hasIsMove = Animator.StringToHash("IsMove");
        protected int _hasMoveSpeed = Animator.StringToHash("MoveSpeed");

        public bool IsPatrol { get { return _isPatrol; } set { _isPatrol = value; } }

        public override void OnInitialized()
        {
            _animator = context.GetComponent<Animator>();
            _controller = context.GetComponent<CharacterController>();
        }

        public override void OnEnter()
        {
            _animator?.SetBool(_hasIsMove, false);
            _animator?.SetFloat(_hasMoveSpeed, 0);
            _controller?.Move(Vector3.zero);

            if (context is EnemyController_Patrol)
            {
                _isPatrol = true;
                _idleTime = Random.Range(_minIdleTime, _maxIdleTime);
            }
        }

        public override void Update(float deltaTime)
        {
            if (context.AttackTarget)
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
            else if (_isPatrol && stateMachine.ElpasedTimeInState > _idleTime)
            {
                stateMachine.ChangeState<MoveToWayPointState>();
            }
        }

        public override void OnExit()
        {
        }
    }
}