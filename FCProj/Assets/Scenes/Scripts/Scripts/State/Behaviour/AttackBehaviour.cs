using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBehaviour : MonoBehaviour
{
#region Variables
  
#if UNITY_EDITOR
    // 에디터에 표시될 설명이 필요한 경우에 사용...
    [Multiline]
    public string developmentDescription = "";
#endif

    public int _animationIndex;
    [SerializeField] protected int _priority;

    [SerializeField] protected int _damage = 10;
    [SerializeField] protected float _range = 3f;

    [SerializeField] protected float _coolTime;

    // 쿨타임 경과 시간.
    [SerializeField] protected float _calcCoolTime = 0.0f;

    // 피격 시 사용할 이펙트
    [SerializeField] protected GameObject _effectPrefab;

    [HideInInspector]
    public LayerMask _targetMask;
    #endregion


    #region Properties
    public bool IsAvailable => _calcCoolTime >= _coolTime;
    public LayerMask TargetMask { get { return _targetMask; } set { _targetMask = value; } }
    public float Range => _range;
    public int Priority => _priority;

    public int Damage => _damage;
    #endregion

    private void Start()
    {
        _calcCoolTime = _coolTime;
    }

    private void Update()
    {
        if(_calcCoolTime < _coolTime)
        {
            _calcCoolTime += Time.deltaTime;
        }
    }

    // 공격 대상, 범위 공격인 경우 null
    public abstract void ExecuteAttack(GameObject target = null, Transform startPoint = null);
}
