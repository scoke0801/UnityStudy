using UnityEditor;
using UnityEngine;

namespace State
{
    public class EnemyController : MonoBehaviour
    {
        protected StateMachineEx<EnemyController> _stateMachine;
        public StateMachineEx<EnemyController> StateMachine => _stateMachine;

        [SerializeField] private LayerMask _searchLayerMask; // 탐색 대상 레이어 마스크
        [SerializeField] private float _viewRadius;     // 시야 범위
        [SerializeField] private Transform _attackTarget;     // 공격 대상
        [SerializeField] private float _attackRange;    // 공격 범위

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
        #endregion

        #region UnityEvents
        private void Start()
        {
            _stateMachine = new StateMachineEx<EnemyController>(this, new IdleState());

            _stateMachine.AddState(new MoveState());
            _stateMachine.AddState(new AttackState());
        }

        private void Update()
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

        #endregion
    }
}