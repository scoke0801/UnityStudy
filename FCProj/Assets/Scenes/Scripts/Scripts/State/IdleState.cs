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

        protected int _hasMove = Animator.StringToHash("Move");
        protected int _hasMoveSpeed = Animator.StringToHash("MoveSpeed");

        public bool IsPatrol { get { return _isPatrol; } set { _isPatrol = value; } }

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

            if (_isPatrol)
            {
                _idleTime = Random.Range(_minIdleTime, _maxIdleTime);
            }
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
            else if(_isPatrol && stateMachine.ElpasedTimeInState > _idleTime)
            {
                stateMachine.ChangeState<MoveToWayPointState>();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

    }
}