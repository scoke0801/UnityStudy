using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace State
{
    public class MoveState : State<EnemyContorller>
    {
        private Animator _animator;
        private CharacterController _controller;
        private NavMeshAgent _agent;

        private int _hashMove = Animator.StringToHash("Move");
        private int _hashMoveSpeed = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized()
        {
            _animator = context.GetComponent<Animator>();
            _controller = context.GetComponent<CharacterController>();
            _agent = context.GetComponent<NavMeshAgent>();
        }

        public override void OnEnter()
        {
            _agent?.SetDestination(context.AttackTarget.position);
            _animator.SetBool(_hashMove, true);
        }

        public override void Update(float deltaTime)
        {
            Transform enemy = context.SearchEnemy();

            if (enemy)
            {
                _agent.SetDestination(context.AttackTarget.position);

                if(_agent.remainingDistance > _agent.stoppingDistance)
                {
                    _controller.Move(_agent.velocity * deltaTime);

                    // 3번째 인자는 애니메이션을 damp 보간하도록 지정.
                    _animator.SetFloat(_hashMoveSpeed, _agent.velocity.magnitude / _agent.speed, 1f, deltaTime);
                }
            }

            // 적이 없거나 이미 목표지점에 도달한 경우.
            if(!enemy && _agent.remainingDistance > _agent.stoppingDistance)
            {
                stateMachine.ChangeState<IdleState>();
            }
        }

        public override void OnExit()
        {
            // 그만 이동하도록 상태를 설정.
            _animator?.SetBool(_hashMove, false);
            _animator?.SetFloat(_hashMoveSpeed, 0.0f);
            _agent.ResetPath();
        }
    }

}