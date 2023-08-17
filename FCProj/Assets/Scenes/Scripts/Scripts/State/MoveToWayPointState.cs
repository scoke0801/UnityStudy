using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace State
{
    public class MoveToWayPointState : State<EnemyController>
    {
        private Animator _animator;
        private CharacterController _controller;
        private NavMeshAgent _agent;

        protected int _hashMove = Animator.StringToHash("Move");
        protected int _hasMoveSpeed = Animator.StringToHash("MoveSpeed");


        public override void OnInitialized()
        {
            _animator = context.GetComponent<Animator>();
            _controller = context.GetComponent<CharacterController>();
            _agent = context.GetComponent<NavMeshAgent>();
        }

        public override void OnEnter()
        {
            if(context.TargetWayPoint == null)
            {
                context.FindNextWayPoint();
            }

            if(context.TargetWayPoint)
            {
                _agent?.SetDestination(context.TargetWayPoint.position);
                _animator?.SetBool(_hashMove, true);
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
            else
            {
                // pathPending: 이동할 경로가 있는 지.
                if(!_agent.pathPending && (_agent.remainingDistance <= _agent.stoppingDistance))
                {
                    context.FindNextWayPoint();
                    stateMachine.ChangeState<IdleState>();
                }
                else
                {
                    _controller.Move(_agent.velocity * deltaTime);
                    _animator.SetFloat(_hasMoveSpeed, _agent.velocity.magnitude / _agent.speed, .1f, deltaTime);
                }
            }
        }

        public override void OnExit()
        {
            _animator?.SetBool(_hashMove, false);
            _agent.ResetPath();
        }

    }
}