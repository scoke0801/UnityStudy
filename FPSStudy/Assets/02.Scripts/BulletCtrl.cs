using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    // �Ѿ��� �ı���
    public float damage = 20.0f;

    // �Ѿ� �߻� ��.
    public float force = 1500.0f;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _rigidbody.AddForce(transform.forward * force);

        // ���� ��ǥ�� ������ ���� �������� ���� ���Ѵ�.
        //_rigidbody.AddRelativeForce(Vector3.forward * force);
    }
}
