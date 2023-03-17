using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sword : Weapon
{
    override protected void Update()
    {
        base.Update();

        Debug.Log("Sword Update");
    }
}
