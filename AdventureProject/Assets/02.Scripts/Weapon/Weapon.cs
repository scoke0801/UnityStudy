using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected int comboCount = 0;
    protected WeaponInfo info;

    private void OnEnable()
    {
        // TryGetComponent(out info);
    }

    virtual protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("AttackTest");
        }
    }
}
