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

        [SerializeField] private Transform _targetWaypoint = null;
        [SerializeField] private int _wayPointIndex = 0;

        protected int _hashIsMove = Animator.StringToHash("IsMove");
        protected int _hasMoveSpeed = Animator.StringToHash("MoveSpeed");

        private EnemyController_Patrol _patrolController;

        private Transform[] Waypoints => ((EnemyController_Patrol)context)?.WayPoints;

        public override void OnInitialized()
        {
            _animator = context.GetComponent<Animator>();
            _controller = context.GetComponent<CharacterController>();
            _agent = context.GetComponent<NavMeshAgent>();

            _patrolController = context as EnemyController_Patrol;
        }

        public override void OnEnter()
        {
            _agent.stoppingDistance = 0.0f;

            if (_targetWaypoint == null)
            {
                FindNextWayPoint();
            }

            if (_targetWaypoint)
            {
                _animator?.SetBool(_hashIsMove, true);
                _agent.SetDestination(_targetWaypoint.position);
            }
            else
            {
                stateMachine.ChangeState<IdleState>();
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
            else
            {

                if (!_agent.pathPending && (_agent.remainingDistance <= _agent.stoppingDistance))
                {
                    FindNextWayPoint();
                    stateMachine.ChangeState<IdleState>();
                }
                else
                {
                    _controller.Move(_agent.velocity * Time.deltaTime);
                    _animator.SetFloat(_hasMoveSpeed, _agent.velocity.magnitude / _agent.speed, .1f, Time.deltaTime);
                }
            }
        }

        public override void OnExit()
        {
            _agent.stoppingDistance = context.AttackRange;
            _animator?.SetBool(_hashIsMove, false);
            _agent.ResetPath();
        }

        public Transform FindNextWayPoint()
        {
            _targetWaypoint = null;

            if (Waypoints != null && Waypoints.Length > 0)
            {
                _targetWaypoint = Waypoints[_wayPointIndex];

                _wayPointIndex = (_wayPointIndex + 1) % Waypoints.Length;
            }

            return _targetWaypoint;
        }
    }
}