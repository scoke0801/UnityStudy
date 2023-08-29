using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace State
{
    public class EnemyController_Range : EnemyController, IAttackable, IDamageable
    {
        #region Variables
        private float _health;
        private Transform _hitPoint;

        private int _hitTriggerHash = Animator.StringToHash("HitTrigger");

        [SerializeField] private Transform _projectilePoint;
        #endregion Variables

        #region Properties
        public float MaxHealth => 100f;
        public override float AttackRange => CurrentAttackBehaviour?.Range ?? 6.0f;

        public override bool IsAvailableAttack
        {
            get
            {
                if (!AttackTarget)
                {
                    return false;
                }

                float distance = Vector3.Distance(transform.position, AttackTarget.position);
                return (distance <= AttackRange);
            }
        }
        #endregion

        #region Unity Methods

        protected override void Start()
        {
            base.Start();

            _stateMachine.AddState(new MoveState());
            _stateMachine.AddState(new AttackState());
            _stateMachine.AddState(new DeadState());

            _health = MaxHealth;

            if (_battleUI)
            {
                _battleUI.MinimumValue = 0.0f;
                _battleUI.MaximumValue = MaxHealth;
                _battleUI.Value = _health;
            }

            InitAttackBehaviour();
        }

        protected override void Update()
        {
            CheckAttackBehaviour();

            base.Update();
        }

        private void OnAnimatorMove()
        {
            Vector3 position = transform.position;
            position.y = _agent.nextPosition.y;

            _animator.rootPosition = position;
            _agent.nextPosition = position;
        }

        #endregion Unity Methods

        #region Helper Methods
        private void InitAttackBehaviour()
        {
            foreach (AttackBehaviour behaviour in attackBehaviours)
            {
                if (CurrentAttackBehaviour == null)
                {
                    CurrentAttackBehaviour = behaviour;
                }

                behaviour.TargetMask = TargetMask;
            }
        }

        private void CheckAttackBehaviour()
        {
            if (CurrentAttackBehaviour == null || !CurrentAttackBehaviour.IsAvailable)
            {
                CurrentAttackBehaviour = null;

                foreach (AttackBehaviour behaviour in attackBehaviours)
                {
                    if (behaviour.IsAvailable)
                    {
                        if ((CurrentAttackBehaviour == null) || (CurrentAttackBehaviour.Priority < behaviour.Priority))
                        {
                            CurrentAttackBehaviour = behaviour;
                        }
                    }
                }
            }
        }

        #endregion Helper Methods

        #region IDamagable interfaces

        public bool IsAlive => (_health > 0);

        public void TakeDamage(int damage, GameObject hitEffectPrefab)
        {
            if (!IsAlive)
            {
                return;
            }

            _health -= damage;

            if (_battleUI)
            {
                _battleUI.Value = _health;
                _battleUI.TakeDamage(damage);
            }

            if (hitEffectPrefab)
            {
                Instantiate(hitEffectPrefab, _hitPoint);
            }

            if (IsAlive)
            {
                _animator?.SetTrigger(_hitTriggerHash);
            }
            else
            {
                if (_battleUI != null)
                {
                    _battleUI.enabled = false;
                }

                _stateMachine.ChangeState<DeadState>();
            }
        }

        #endregion IDamagable interfaces

        #region IAttackable Interfaces


        [SerializeField]
        private List<AttackBehaviour> attackBehaviours = new List<AttackBehaviour>();

        public AttackBehaviour CurrentAttackBehaviour
        {
            get;
            private set;
        }

        public void OnExecuteAttack(int attackIndex)
        {
            if (CurrentAttackBehaviour != null && AttackTarget != null)
            {
                CurrentAttackBehaviour.ExecuteAttack(AttackTarget.gameObject, _projectilePoint);
            }
        }

        #endregion IAttackable Interfaces
    }
}