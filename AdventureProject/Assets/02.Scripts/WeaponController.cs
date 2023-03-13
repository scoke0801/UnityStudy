using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponInfo _weaponInfo;
    [SerializeField] private Transform _hand;
    
    private GameObject _weapon;
    private Collider _collider;

    private void Awake()
    {
        SetWeapon(_weaponInfo);
    }

    public void SetWeapon(WeaponInfo weaponInfo)
    {
        if (!weaponInfo) { return; }
        if (!_hand) { return; }

        if (!weaponInfo.IsValid()) { return; }

        _weaponInfo = weaponInfo;
        // 기존에 장착중이던 무기는 파괴.
        if ( _weapon) { Destroy(_weapon); }

        Debug.Log(weaponInfo.name);

        _weapon = GameObject.Instantiate(weaponInfo.Prefab, _hand.transform);
        _collider = _weapon.GetComponentInChildren<Collider>();

        _weapon.transform.SetLocalPositionAndRotation(weaponInfo.Position, weaponInfo.Rotation);
        _weapon.transform.localScale = weaponInfo.Scale;
    }

    public void OnAttack()
    {
        if (!_weapon || !_collider) { return; }

        StartCoroutine(nameof(Attack));
    }

    private IEnumerator Attack()
    {
        _collider.enabled = true;

        yield return new WaitForSeconds(1f);

        _collider.enabled = false;
    }
}
