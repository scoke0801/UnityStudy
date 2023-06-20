using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ArrowShooter : MonoBehaviour
{
    [SerializeField] GameObject _prefab;
    [SerializeField] float _speed = 10.0f;
    [SerializeField] private WeaponInfo _arrowWeaponInfo;
    [SerializeField] Animator _animator;
    [SerializeField] Transform _spine;
    [SerializeField] Quaternion _spineOffset;
    [SerializeField] Transform _bowString;
    [SerializeField] Transform _leftHand;

    private Vector3 _bowStringOrigin;
    private Camera _aimCamera;
    private WeaponController _weaponController;
    
    private Transform _shooter;
    private GameObject _arrow;
    private Vector3 _direction;

    private float _arrowDestroyTime = 10.0f;
    private float _arrowCreateTime = 1.0f;

    private bool _isFired = false;
    
    bool _isAiming = false;

    private void Awake()
    {
        _aimCamera = GetComponentInChildren<Camera>();

        _shooter = gameObject.transform;
        _weaponController = GetComponent<WeaponController>();

        _animator = GetComponent<Animator>();

        if (_bowString)
        {
            _bowStringOrigin = _bowString.localPosition;
        }
    }
    private void Update()
    {
        if( _isAiming )
        {
            _bowString.position = _leftHand.position;
        }
        else
        {
            _bowString.position = _bowStringOrigin;
        }

        if (!_arrow || !_isFired )
        {
            return;
        }

        _arrow.transform.position += _direction.normalized * _speed * Time.deltaTime;
    }

    public void Attack( GameObject arrow )
    {
        if (_isAiming)
        {
            _animator.SetTrigger("ArrowShoot");
            _isAiming = false;
            _isFired = true;

            _arrow.transform.SetParent(null);

            _direction = transform.forward;
            
            GameObject.Destroy(_arrow, _arrowDestroyTime);

            StartCoroutine(nameof(SetNewArrowRoutine));
        }
        else
        {
            _animator.SetTrigger("BowAttack");
 
            _arrow = arrow;
        }
    }

    void OnArrowAim()
    {
        if (!_arrow) { return; }

        _isAiming = true;
    }

    IEnumerator SetNewArrowRoutine()
    {
        yield return new WaitForSeconds(_arrowCreateTime);

        _isFired = false;
        _weaponController.SetLeftHandWeapon(_arrowWeaponInfo);
    }
}
