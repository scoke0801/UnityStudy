using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace State
{
    public class EnemyController_Patrol : EnemyController, IDamageable
    {
        #region Variables

        [SerializeField] private Collider _weaponCollider;
        [SerializeField] private Transform _hitPoint;
        [SerializeField] private GameObject _hitEffect = null;

        [SerializeField] private Transform[] _waypoints;

        [SerializeField] private NPCBattleUI _healthBar;

        #endregion Variables

        #region Proeprties
        public Transform[] WayPoints => _waypoints;
        #endregion Properties

        #region Unity Methods
        protected override void Start()
        {
            base.Start();

            _stateMachine.AddState(new MoveState());
            _stateMachine.AddState(new MoveToWayPointState());

            health = maxHealth;

            if (_healthBar)
            {
                _healthBar = GetComponent<NPCBattleUI>();
                _healthBar.MinimumValue = 0.0f;
                _healthBar.MaximumValue = maxHealth;
                _healthBar.Value = health;
            }
        }
        #endregion Unity Methods

        #region Helper Methods

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

        public void EnableAttackCollider()
        {
            Debug.Log("Check Attack Event");
            if (_weaponCollider)
            {
                _weaponCollider.enabled = true;
            }

            StartCoroutine("DisableAttackCollider");
        }

        IEnumerator DisableAttackCollider()
        {
            yield return new WaitForFixedUpdate();

            if (_weaponCollider)
            {
                _weaponCollider.enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other != _weaponCollider)
            {
                return;
            }

            if (((1 << other.gameObject.layer) & TargetMask) != 0)
            {
                //It matched one
                Debug.Log("Attack Trigger: " + other.name);
                PlayerCharacter playerCharacter = other.gameObject.GetComponent<PlayerCharacter>();
                playerCharacter?.TakeDamage(10, _hitEffect);

            }

            if (((1 << other.gameObject.layer) & TargetMask) == 0)
            {
                //It wasn't in an ignore layer
            }
        }

        #endregion Helper Methods

        #region IDamagable interfaces

        public float maxHealth = 100f;

        private float health;

        public bool IsAlive => (health > 0);

        private int _hitTriggerHash = Animator.StringToHash("HitTrigger");
        private int _isAliveHash = Animator.StringToHash("IsAlive");

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
                _healthBar.enabled = false;
                _animator?.SetBool(_isAliveHash, false);

                Destroy(gameObject, 3.0f);
            }
        }
        #endregion IDamagable interfaces
    }
}