using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace State
{
    public class EnemyController : MonoBehaviour
    {
        #region Variables
        protected StateMachineEx<EnemyController> _stateMachine;
        public StateMachineEx<EnemyController> StateMachine => _stateMachine;

        [SerializeField] private NPCBattleUI _battleUI;

        protected NavMeshAgent _agent;
        protected Animator _animator;
        private ObjectFieldOfView _fieldOfView;

        [SerializeField] private Transform[] _wayPoints;
        private Transform _targetWayPoint = null;

        private int _currentWayPointIndex = 0;

        public int _maxHealth;
        #endregion

        #region Properties 
        public Transform AttackTarget { get { return _fieldOfView.NearestTarget; } }
        public Transform TargetWayPoint => _targetWayPoint;
        public LayerMask TargetMask => _fieldOfView.TargetMask;
        public int Health { get; set; }

        public virtual float AttackRange => 3.0f;
        public virtual bool IsAvailableAttack => false;
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
            if(!(_stateMachine.CurrentState is MoveState) && !(_stateMachine.CurrentState is DeadState))
            {
                FaceTarget();
            }
        }

        void FaceTarget()
        {
            if (AttackTarget)
            {
                Vector3 direction = (AttackTarget.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
        private void OnAnimatorMove()
        {
            Vector3 pos = transform.position;
            pos.y = _agent.nextPosition.y;

            _animator.rootPosition = pos;
            _agent.nextPosition = pos;
        }
        #endregion

        #region Public Methods
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