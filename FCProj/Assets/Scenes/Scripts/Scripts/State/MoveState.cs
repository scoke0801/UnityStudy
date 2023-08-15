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

                    // 3��° ���ڴ� �ִϸ��̼��� damp �����ϵ��� ����.
                    _animator.SetFloat(_hashMoveSpeed, _agent.velocity.magnitude / _agent.speed, 1f, deltaTime);
                }
            }

            // ���� ���ų� �̹� ��ǥ������ ������ ���.
            if(!enemy && _agent.remainingDistance > _agent.stoppingDistance)
            {
                stateMachine.ChangeState<IdleState>();
            }
        }

        public override void OnExit()
        {
            // �׸� �̵��ϵ��� ���¸� ����.
            _animator?.SetBool(_hashMove, false);
            _animator?.SetFloat(_hashMoveSpeed, 0.0f);
            _agent.ResetPath();
        }
    }

}