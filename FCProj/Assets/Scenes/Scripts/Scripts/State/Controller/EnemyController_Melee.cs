using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

namespace State
{
    public class EnemyController_Melee : EnemyController, IAttackable, IDamageable
    {
        #region Variables
        [SerializeField] private Transform _hitPoint;

        [SerializeField] private NPCBattleUI _healthBar;

        [SerializeField] private Transform[] _waypoints;

        private float health;

        private int _hitTriggerHash = Animator.StringToHash("HitTrigger");
        private int _isAliveHash = Animator.StringToHash("IsAlive");

        #endregion Variables

        #region Proeprties
        public float MaxHealth => 100f;
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

        #endregion Properties

        #region Unity Methods

        protected override void Start()
        {
            base.Start();

            _stateMachine.AddState(new MoveState());
            _stateMachine.AddState(new AttackState());
            _stateMachine.AddState(new DeadState());

            health = MaxHealth;

            if (_healthBar)
            {
                _healthBar.MinimumValue = 0.0f;
                _healthBar.MaximumValue = MaxHealth;
                _healthBar.Value = health;
            }
        }

        private void OnAnimatorMove()
        {
            // Follow CharacterController
            Vector3 position = transform.position;
            position.y = _agent.nextPosition.y;

            _animator.rootPosition = position;
            _agent.nextPosition = position;
        }

        #endregion Unity Methods

        #region Helper Methods
        #endregion Helper Methods

        #region IDamagable interfaces

        public bool IsAlive => (health > 0);

        public void TakeDamage(int damage, GameObject hitEffectPrefab)
        {
            if (!IsAlive)
            {
                return;
            }

            health -= damage;

            if (_healthBar)
            {
                _healthBar.Value = health;
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
                if (_healthBar != null)
                {
                    _healthBar.enabled = false;
                }

                _stateMachine.ChangeState<DeadState>();
            }
        }

        #endregion IDamagable interfaces

        #region IAttackable Interfaces

        public GameObject hitEffectPrefab = null;

        [SerializeField]
        private List<AttackBehaviour> attackBehaviours = new List<AttackBehaviour>();

        public AttackBehaviour CurrentAttackBehaviour
        {
            get;
            private set;
        }

        public void OnExecuteAttack(int attackIndex)
        {

        }

        #endregion IAttackable Interfaces
    }
}