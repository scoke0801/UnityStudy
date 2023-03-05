using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    public GameObject explosionEffect;

    private Transform tr;
    private Rigidbody rb;

    public Texture[] textures;
    private new MeshRenderer renderer;

    private int hitCount = 0;

    public float radius = 10.0f;

    Collider[] explosionTargets = new Collider[10];

    private void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        renderer = GetComponentInChildren<MeshRenderer>();

        int idx = Random.Range(0, textures.Length);
        renderer.material.mainTexture = textures[idx];
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

        //rb.mass = 1.0f;
        //rb.AddForce(Vector3.up * 1500.0f);

        IndirectDamage(tr.position);

        Destroy(gameObject, 3.0f);
    }

    void IndirectDamage(Vector3 pos)
    {
        Physics.OverlapSphereNonAlloc(pos, radius, explosionTargets, 1 << 3);

        foreach(var coll in explosionTargets)
        {
            rb = coll.GetComponent<Rigidbody>();

            rb.mass = 1.0f;

            // Freeze rotaion «ÿ¡¶.
            rb.constraints = RigidbodyConstraints.None;

            rb.AddExplosionForce(1500.0f, pos, radius, 1200.0f);

        }
    }

}

