using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponInfo _weaponInfo;
    [SerializeField] private Transform _hand;
    
    private GameObject _weapon;

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

        _weapon.transform.SetLocalPositionAndRotation(weaponInfo.Position, weaponInfo.Rotation);
        _weapon.transform.localScale = weaponInfo.Scale;
    }
}
