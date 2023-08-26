using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Follow : Projectile
{
    public float _destroyDelay = 5.0f;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(DestroyParticle(_destroyDelay));
    }

    protected override void FixedUpdate()
    {
        if (_target)
        {
            Vector3 dest = _target.transform.position;

            dest.y += 1.5f;

            transform.LookAt(dest);
        }

        base.FixedUpdate();
    }
}
