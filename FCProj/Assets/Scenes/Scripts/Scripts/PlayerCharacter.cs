using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Animator))]
public class PlayerCharacter : MonoBehaviour, IAttackable, IDamageable
{
    #region Variables
    public TargetPicker _picker;

    private CharacterController _controller;

    [SerializeField] private LayerMask groundLayerMask;

    private NavMeshAgent _agent;
    private Camera _camera;

    [SerializeField] private Animator _animator;

    readonly int _moveHash = Animator.StringToHash("Move");
    readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");
    readonly int _fallingHash = Animator.StringToHash("Falling");
    readonly int _attackTriggerHash = Animator.StringToHash("AttackTrigger");
    readonly int _attackIndexHash = Animator.StringToHash("AttackIndex");
    readonly int _hitTriggerHash = Animator.StringToHash("HitTrigger");
    readonly int _isAliveHash = Animator.StringToHash("IsAlive");

    [SerializeField] private LayerMask targetMask;

    public Transform target;

    public float maxHealth = 100f;
    protected float health;

    [SerializeField] private Transform hitPoint;
    #endregion

    #region Properties
    public bool IsInAttackState => GetComponent<AttackStateController>()?.IsInAttackState ?? false;
    #endregion

    #region Main Methods
    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();

        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _agent.updateRotation = true;

        _camera = Camera.main;

        health = maxHealth;

        InitAttackBehaviour();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsAlive)
        {
            return;
        }

        CheckAttackBehaviour();

        // Process mouse left button input
        if (Input.GetMouseButtonDown(0) && !IsInAttackState)
        {
            // Make ray from screen to world
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            // Check hit from ray
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, groundLayerMask))
            {
                Debug.Log("We hit " + hit.collider.name + " " + hit.point);

                RemoveTarget();

                // Move our player to what we hit
                _agent.SetDestination(hit.point);

                if (_picker)
                {
                    _picker.SetPosition(hit);
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // Make ray from screen to world
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            // Check hit from ray
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log("We hit " + hit.collider.name + " " + hit.point);

                IDamageable damagable = hit.collider.GetComponent<IDamageable>();
                if (damagable != null && damagable.IsAlive)
                {
                    SetTarget(hit.collider.transform);

                    if (_picker)
                    {
                        _picker.Target = hit.collider.transform;
                    }
                }
            }
        }

        if (target != null)
        {
            if (!(target.GetComponent<IDamageable>()?.IsAlive ?? false))
            {
                RemoveTarget();
            }
            else
            {
                _agent.SetDestination(target.position);
                FaceToTarget();
            }
        }

        if ((_agent.remainingDistance > _agent.stoppingDistance))
        {
            _controller.Move(_agent.velocity * Time.deltaTime);
            _animator.SetFloat(_moveSpeedHash, _agent.velocity.magnitude / _agent.speed, .1f, Time.deltaTime);
            _animator.SetBool(_moveHash, true);
        }
        else
        {
            _controller.Move(_agent.velocity * Time.deltaTime);

            if (!_agent.pathPending)
            {
                _animator.SetFloat(_moveSpeedHash, 0);
                _animator.SetBool(_moveHash, false);
                _agent.ResetPath();
            }
        }

        //calcAttackCoolTime += Time.deltaTime;
        AttackTarget();
    }

    private void OnAnimatorMove()
    {
        Vector3 position = transform.position;
        position.y = _agent.nextPosition.y;

        _animator.rootPosition = position;
        _agent.nextPosition = position;
    }
    #endregion Main Methods

    #region Helper Methods
    private void InitAttackBehaviour()
    {
        foreach (AttackBehaviour behaviour in attackBehaviours)
        {
            behaviour.TargetMask = targetMask;
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

    void SetTarget(Transform newTarget)
    {
        target = newTarget;

        _agent.stoppingDistance = CurrentAttackBehaviour?.Range ?? 0;
        _agent.updateRotation = false;
        _agent.SetDestination(newTarget.transform.position);
    }

    void RemoveTarget()
    {
        target = null;

        _agent.stoppingDistance = 0f;
        _agent.updateRotation = true;

        _agent.ResetPath();
    }

    void AttackTarget()
    {
        if (CurrentAttackBehaviour == null)
        {
            return;
        }

        if (target != null && !IsInAttackState && CurrentAttackBehaviour.IsAvailable)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= CurrentAttackBehaviour?.Range)
            {
                _animator.SetInteger(_attackIndexHash, CurrentAttackBehaviour._animationIndex);
                _animator.SetTrigger(_attackTriggerHash);

            }
        }
    }

    void FaceToTarget()
    {
        if (target)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10.0f);
        }
    }
    #endregion Helper Methods

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
        if (CurrentAttackBehaviour != null)
        {
            //   CurrentAttackBehaviour.ExecuteAttack(.gameObject);
        }
    }
    #endregion IAttackable Interfaces

    #region IDamagable Interfaces
    public bool IsAlive => health > 0;
    public void TakeDamage(int damage, GameObject hitEffectPrefab)
    {
        if (!IsAlive)
        {
            return;
        }

        health -= damage;

        if (hitEffectPrefab)
        {
            Instantiate<GameObject>(hitEffectPrefab, hitPoint);
        }

        if (IsAlive)
        {
            _animator?.SetTrigger(_hitTriggerHash);
        }
        else
        {
            _animator?.SetBool(_isAliveHash, false);
        }
    }
    #endregion IDamagable Interfaces
}
