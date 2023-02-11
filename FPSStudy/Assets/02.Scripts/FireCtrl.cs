using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FireCtrl : MonoBehaviour
{
    public GameObject bullet;

    public Transform firePos;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    void Fire()
    {
        Instantiate(bullet, firePos.position, firePos.rotation);
    }
}

