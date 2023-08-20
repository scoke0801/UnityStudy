using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace State
{
    public class EnemyController : MonoBehaviour
    {
        protected StateMachineEx<EnemyController> _stateMachine;
        public StateMachineEx<EnemyController> StateMachine => _stateMachine;

        [SerializeField] protected LayerMask _searchLayerMask; // 탐색 대상 레이어 마스크
        [SerializeField] private float _viewRadius;     // 시야 범위
        [SerializeField] private Transform _attackTarget;     // 공격 대상
        [SerializeField] private float _attackRange;    // 공격 범위

        [SerializeField] private Transform[] _wayPoints;
        private Transform _targetWayPoint = null;

        private int _currentWayPointIndex = 0;

        #region Properties
        public bool IsAvailableAttack
        {
            get
            {
                if (_attackTarget)
                {
                    return false;
                }

                float distance = Vector3.Distance(transform.position, _attackTarget.position);
                return distance <= _attackRange;
            }
        }

        public Transform AttackTarget { get { return _attackTarget; } }
        public Transform TargetWayPoint => _targetWayPoint;
        #endregion

        #region UnityEvents
        protected virtual void Start()
        {
            _stateMachine = new StateMachineEx<EnemyController>(this, new MoveToWayPointState());
            IdleState idleState = new IdleState();
            idleState.IsPatrol = true;

            _stateMachine.AddState(idleState);
            _stateMachine.AddState(new MoveState());
            _stateMachine.AddState(new AttackState());
        }

        protected virtual void Update()
        {
            _stateMachine.Update(Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            // 시야 및 공격 범위만큼의 기즈모를 표시.
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _viewRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
        #endregion

        #region Public Methods
        public Transform SearchEnemy()
        {
            _attackTarget = null;

            Collider[] targetInViewRadius = Physics.OverlapSphere(
                transform.position, _viewRadius, _searchLayerMask);

            if (targetInViewRadius.Length > 0)
            {
                _attackTarget = targetInViewRadius[0].transform;
            }

            return _attackTarget;
        }
        public R ChangeState<R>() where R : State<EnemyController>
        {
            return _stateMachine.ChangeState<R>();
        }

        public Transform FindNextWayPoint()
        {
            // 탐색 전 초기화
            _targetWayPoint = null;

            if(_wayPoints.Length > 0)
            {
                _targetWayPoint = _wayPoints[_currentWayPointIndex];
            }

            _currentWayPointIndex = (_currentWayPointIndex + 1) % _wayPoints.Length;

            return _targetWayPoint;
        }
        #endregion
    }
}