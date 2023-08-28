using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBehaviour : MonoBehaviour
{
#region Variables
  
#if UNITY_EDITOR
    // �����Ϳ� ǥ�õ� ������ �ʿ��� ��쿡 ���...
    [Multiline]
    public string developmentDescription = "";
#endif

    public int _animationIndex;
    [SerializeField] protected int _priority;

    [SerializeField] protected int _damage = 10;
    [SerializeField] protected float _range = 3f;

    [SerializeField] protected float _coolTime;

    // ��Ÿ�� ��� �ð�.
    [SerializeField] protected float _calcCoolTime = 0.0f;

    // �ǰ� �� ����� ����Ʈ
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

    // ���� ���, ���� ������ ��� null
    public abstract void ExecuteAttack(GameObject target = null, Transform startPoint = null);
}
