using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    // 총알의 파괴력
    public float damage = 20.0f;

    // 총알 발사 힘.
    public float force = 1500.0f;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.AddForce(transform.forward * force);

        // 로컬 좌표계 기준의 전진 방향으로 힘을 가한다.
        //_rigidbody.AddRelativeForce(Vector3.forward * force);
    }
}
