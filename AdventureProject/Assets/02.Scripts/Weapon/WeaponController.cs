using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponInfo _leftWeaponInfo;
    [SerializeField] private WeaponInfo _rightWeaponInfo;

    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;

    private GameObject _leftHandWeapon;
    private GameObject _rightHandWeapon;

    private Collider _collider;

    public WeaponInfo Info {  get { return _rightWeaponInfo; } }

    private ArrowShooter _arrowShooter;

    private void Awake()
    {
        if(_leftWeaponInfo)
        {
            SetLeftHandWeapon(_leftWeaponInfo);
        }

        if (_rightWeaponInfo)
        {
            SetRightHandWeapon(_rightWeaponInfo);
        }

        _arrowShooter = GetComponent<ArrowShooter>();
    }

    public void SetLeftHandWeapon(WeaponInfo weaponInfo)
    {
        if (!weaponInfo) { return; }

        if (!_leftHand) { return; }

        if (!weaponInfo.IsValid()) { return; }

        _leftWeaponInfo = weaponInfo;
        // 기존에 장착중이던 무기는 파괴.
        if ( _leftHandWeapon) { Destroy(_leftHandWeapon); }

        Debug.Log(weaponInfo.name);

        _leftHandWeapon = GameObject.Instantiate(weaponInfo.Prefab, _leftHand.transform);
        // _collider = _leftHandWeapon.GetComponentInChildren<Collider>();

        _leftHandWeapon.transform.SetLocalPositionAndRotation(weaponInfo.Position, weaponInfo.Rotation);
        _leftHandWeapon.transform.localScale = weaponInfo.Scale;
    }

    public void SetRightHandWeapon(WeaponInfo weaponInfo)
    {
        if (!weaponInfo) { return; }

        if (!_rightHand) { return; }

        if (!weaponInfo.IsValid()) { return; }

        _rightWeaponInfo = weaponInfo;
        // 기존에 장착중이던 무기는 파괴.
        if (_rightHandWeapon) { Destroy(_rightHandWeapon); }

        Debug.Log(weaponInfo.name);

        _rightHandWeapon = GameObject.Instantiate(weaponInfo.Prefab, _rightHand.transform);
        _collider = _rightHandWeapon.GetComponentInChildren<Collider>();

        _rightHandWeapon.transform.SetLocalPositionAndRotation(weaponInfo.Position, weaponInfo.Rotation);
        _rightHandWeapon.transform.localScale = weaponInfo.Scale;
    }

    public void OnAttack()
    {
        if (!_leftHandWeapon || !_collider) { return; }

        StartCoroutine(nameof(Attack));
    }

    private IEnumerator Attack()
    {
        _collider.enabled = true;

        if(_rightHandWeapon && _rightWeaponInfo.Category == WeaponCategory.Bow )
        {
            _arrowShooter.Attack();
        }
        yield return new WaitForSeconds(1f);

        _collider.enabled = false;
    }
}
