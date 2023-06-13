using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ArrowShooter : MonoBehaviour
{
    [SerializeField] GameObject _prefab;
    [SerializeField] float _speed = 10.0f;
    [SerializeField] private WeaponInfo _arrowWeaponInfo;

    private Camera _aimCamera;
    private WeaponController _weaponController;
    
    private Transform _shooter;
    private GameObject _arrow;
    private Vector3 _direction;

    private float _arrowDestroyTime = 5.0f;
    private float _arrowCreateTime = 1.0f;

    private bool _isFired = false;

    private void Awake()
    {
        _aimCamera = GetComponentInChildren<Camera>();

        _shooter = gameObject.transform;
        _weaponController = GetComponent<WeaponController>();
    }
    private void Update()
    {
        if (!_arrow || !_isFired )
        {
            return;
        }

        _arrow.transform.position = _arrow.transform.position + _direction.normalized * _speed * Time.deltaTime;
        //var test = _arrow.transform.position;
        //test.z += _speed * Time.deltaTime;
        //_arrow.transform.position = test;
    }

    public void Attack( GameObject arrow )
    {
        // _direction = _shooter.forward.normalized;

        //_direction = arrow.transform.forward;

        _direction = arrow.transform.position;
        _direction.z = 300;

        
        _arrow = arrow;
    }

    void OnArrowAim()
    {
        if (!_arrow) { return; }

        _arrow.transform.SetParent(null);

        GameObject.Destroy(_arrow, _arrowDestroyTime); 

        _arrow.transform.LookAt(_direction);
        _arrow.transform.Rotate(90, 0, 0);

        _isFired = true;

        StartCoroutine(nameof(SetNewArrowRoutine));
    }

    IEnumerator SetNewArrowRoutine()
    {
        yield return new WaitForSeconds(_arrowCreateTime);

        _isFired = false;
        _weaponController.SetLeftHandWeapon(_arrowWeaponInfo);
    }
}
