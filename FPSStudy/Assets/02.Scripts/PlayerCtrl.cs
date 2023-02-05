using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCtrl : MonoBehaviour
{
    private float _moveSpeed = 1.5f;
    private float _turnSpeed = 90.0f;

    private Transform tr;
    private Animation anim;


    private void Start()
    {
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();

        anim.Play("Idle");
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");  // -1.0f ~ 0.0f ~ 1.0f
        float v = Input.GetAxis("Vertical");    // -1.0f ~ 0.0f ~ 1.0f
        float r = Input.GetAxis("Mouse X");

        tr.Translate(Vector3.forward * _moveSpeed * Time.deltaTime * v);

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        tr.Translate(moveDir.normalized * _moveSpeed * Time.deltaTime);

        tr.Rotate(Vector3.up * Time.deltaTime * _turnSpeed * r);
    }
}
