using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    // ���󰡾� �� ����� ������ ����
    public Transform targetTr;

    // main Camera �ڽ��� Transform ������Ʈ
    private Transform camTr;

    // ���� ������κ��� ������ �Ÿ�.
    [Range(2.0f, 20.0f)]
    public float offset = 10.0f;

    // Y������ �̵��� ����.
    [Range(0.0f, 10.0f)]
    public float height = 2.0f;

    // ���� �ӵ�.
    public float damping = 0.1f;

    // ī�޶� LookAt�� offset��.
    public float targetOffset = 2.0f;

    // SmoothDamp���� ����� ����.
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // Main Camera �ڽ��� Transform ������Ʈ�� ����.
        camTr = GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        // �����ؾ� �� ����� �������� offset��ŭ �̵�.
        // ���̸� height��ŭ �̵�.
        Vector3 pos = targetTr.position
            + (-targetTr.forward * offset)
            + (Vector3.up * height);

        // ���� ���� ���� �Լ��� ����� �ε巴�� ��ġ�� ����.
        //camTr.position = Vector3.Slerp(camTr.position,          // ���� ��ġ.
        //    pos,                                                // ��ǥ ��ġ.
        //    Time.deltaTime * damping);                          // �ð� t.

        // SmoothDamp�� �̿��� ��ġ����.
        camTr.position = Vector3.SmoothDamp(camTr.position,   // ���� ��ġ.
            pos,                                              // ��ǥ ��ġ.
            ref velocity,                                     // ���� �ӵ�.
            damping);                                         // ��ǥ ��ġ���� ������ �ð�.

        // Camera�� �ǹ� ��ǥ�� ���� ȸ��.
        camTr.LookAt(targetTr.position + (targetTr.up * targetOffset));
    }
}
