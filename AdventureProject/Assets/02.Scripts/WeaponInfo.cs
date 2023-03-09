using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponCategory
{
    None,
    Sword,
    Bow,
    Staff,
}

[CreateAssetMenu(menuName = "ScriptableObject/Item/Weapon/WeaponInfo", fileName = "WeaponInfo_")]
public class WeaponInfo : ScriptableObject
{
    [SerializeField] private Vector3 position;
    [SerializeField] private Quaternion rotation;
    [SerializeField] private Vector3 scale;

    [SerializeField] private int ID;

    [SerializeField] private WeaponCategory category;

    [SerializeField] private GameObject prefab;

    public GameObject Prefab { get { return prefab; } }
    public Vector3 Position { get { return position; } }
    public Quaternion Rotation { get { return rotation; } }
    public Vector3 Scale { get { return scale; } }

    public bool IsValid()
    {
        if( ID < 0) { return false; }

        if( category == WeaponCategory.None) { return false; }

        if (!prefab) { return false; }
        
        return true;
    }
}
