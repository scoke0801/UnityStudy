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

        protected NavMeshAgent _agent;
        protected Animator _animator;
        private ObjectFieldOfView _fieldOfView;

        [SerializeField] private Transform[] _wayPoints;
        private Transform _targetWayPoint = null;

        private int _currentWayPointIndex = 0;

        public int _maxHealth;
        
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
        public int Health { get; set; }
        #endregion

        #region UnityEvents
        protected virtual void Start()
        {
            _stateMachine = new StateMachineEx<EnemyController>(this, new IdleState());

            _agent = GetComponent<NavMeshAgent>();
            _agent.updatePosition = false;
            _agent.updateRotation = true;

            _animator = GetComponent<Animator>();
            _fieldOfView = GetComponent<ObjectFieldOfView>();
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