using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace State
{
    public class MeleeEnemyController : EnemyController, IAttackable, IDamageable
    {
        #region Variables
        [SerializeField] private List<AttackBehaviour> _attackBehaviours = new List<AttackBehaviour>();
        #endregion

        #region Properties
        [SerializeField] private Transform _projectilePrefab;
        #endregion

        #region UnityEvents
        private void Start()
        {
            _stateMachine.AddState(new MoveState());
            _stateMachine.AddState(new AttackState());
            // _stateMachine.AddState(new DeadState());
        }

        protected override void Update()
        {
            CheckAttackBehaviour();

            base.Update();
        }
        #endregion

        #region Public Methods
        #endregion

        #region IAttackable Interface
        public AttackBehaviour CurrentAttackBehaviour
        {
            get;
            private set;
        }

        public void OnExecuteAttack(int attackIndex)
        {
            if(CurrentAttackBehaviour!=null && AttackTarget != null)
            {
                CurrentAttackBehaviour.ExecuteAttack(AttackTarget.gameObject, _projectilePrefab);
            }
        }
        #endregion

        #region IDamageable Interface
        public bool IsAlive => throw new System.NotImplementedException();

        public void TakeDamage(int damage, GameObject hitEffectPrefabs)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region Private Methods
        private void InitAttackBehaviour()
        {
            foreach(AttackBehaviour behaviour in _attackBehaviours)
            {
                if(CurrentAttackBehaviour == null)
                {
                    CurrentAttackBehaviour = behaviour;
                }

                behaviour._targetMask = _searchLayerMask;
            }
        }

        private void CheckAttackBehaviour()
        {
            if(CurrentAttackBehaviour == null || CurrentAttackBehaviour.IsAvailable == false)
            {
                CurrentAttackBehaviour = null;

                foreach (AttackBehaviour behaviour in _attackBehaviours)
                {
                    if (behaviour.IsAvailable)
                    {
                        // 우선순위가 높은 공격을 탐색
                        if (CurrentAttackBehaviour == null
                            || (CurrentAttackBehaviour._priority < behaviour._priority)){
                            CurrentAttackBehaviour = behaviour;
                        }
                    }
                }
            }
        }
        #endregion
    }
}