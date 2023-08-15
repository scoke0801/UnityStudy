using UnityEditor;
using UnityEngine;

namespace State
{
    public class EnemyContorller : MonoBehaviour
    {
        StateMachineEx<EnemyContorller> _stateMachine;

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
            _stateMachine = new StateMachineEx<EnemyContorller>(this, new IdleState());

            _stateMachine.AddState(new MoveState());
            _stateMachine.AddState(new AttackState());
        }

        private void Update()
        {
            _stateMachine.Update(Time.deltaTime);
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
        #endregion
    }
}