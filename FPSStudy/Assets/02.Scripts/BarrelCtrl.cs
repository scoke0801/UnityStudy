using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    public GameObject explosionEffect;

    private Transform tr;
    private Rigidbody rb;

    private int hitCount = 0;

    private void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BULLET"))
        {
            if( ++hitCount == 3)
            {
                ExplosionBarrel();
            }
        }
    }

    void ExplosionBarrel()
    {
        GameObject particle = Instantiate(explosionEffect, tr.position, Quaternion.identity);
        Destroy(particle, 1.0f);

        rb.mass = 1.0f;
        rb.AddForce(Vector3.up * 1500.0f);

        Destroy(gameObject, 3.0f);
    }

}

