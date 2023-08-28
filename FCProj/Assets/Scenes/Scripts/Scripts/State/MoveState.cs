using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace State
{
    public class MoveState : State<EnemyController>
    {
        private Animator _animator;
        private CharacterController _controller;
        private NavMeshAgent _agent;

        private int _hashIsMove = Animator.StringToHash("IsMove");
        private int _hashMoveSpeed = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized()
        {
            _animator = context.GetComponent<Animator>();
            _controller = context.GetComponent<CharacterController>();
            _agent = context.GetComponent<NavMeshAgent>();
        }

        public override void OnEnter()
        {
            _agent.stoppingDistance = context.AttackRange;
            _agent?.SetDestination(context.AttackTarget.position);

            _animator.SetBool(_hashIsMove, true);
        }

        public override void Update(float deltaTime)
        {
            if (context.AttackTarget)
            {
                _agent.SetDestination(context.AttackTarget.position);
            }

            _controller.Move(_agent.velocity * Time.deltaTime);

            if (_agent.remainingDistance > _agent.stoppingDistance)
            {
                _animator.SetFloat(_hashMoveSpeed, _agent.velocity.magnitude / _agent.speed, .1f, Time.deltaTime);
            }
            else
            {

                if (!_agent.pathPending)
                {
                    _animator.SetFloat(_hashMoveSpeed, 0);
                    _animator.SetBool(_hashIsMove, false);
                    _agent.ResetPath();

                    stateMachine.ChangeState<IdleState>();
                }
            }
        }

        public override void OnExit()
        {
            // 그만 이동하도록 상태를 설정.
            _agent.stoppingDistance = 0.0f;
            _agent.ResetPath();

            _animator?.SetBool(_hashIsMove, false);
            _animator?.SetFloat(_hashMoveSpeed, 0.0f);
        }
    }

}