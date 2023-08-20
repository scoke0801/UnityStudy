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
    public int _priority;

    public int _damage = 10;
    public float _range = 3f;

    [SerializeField]
    protected float _coolTime;

    // ��Ÿ�� ��� �ð�.
    protected float _calcCoolTime = 0.0f;

    // �ǰ� �� ����� ����Ʈ
    public GameObject _effectPrefab;

    [HideInInspector]
    public LayerMask _targetMask;
    #endregion


    #region Properties
    public bool IsAvailable => _calcCoolTime >= _coolTime;
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
