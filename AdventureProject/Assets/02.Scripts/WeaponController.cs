using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private Transform hand;

    private void Awake()
    {
        if (!weaponInfo) { return; }
        if(!hand) { return; }

        if (!weaponInfo.IsValid()) { return; }

        GameObject weapon = GameObject.Instantiate(weaponInfo.Prefab, hand.transform);

        //weapon.transform.SetPositionAndRotation(weaponInfo.Position, weaponInfo.Rotation);

        weapon.transform.SetLocalPositionAndRotation(weaponInfo.Position, weaponInfo.Rotation);
        weapon.transform.localScale = weaponInfo.Scale;
    }

}
