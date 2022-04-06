using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public Transform targetTransform;
    // Start is called before the first frame update
    public Vector3 move = new Vector3(0, 1, 0);
    private void Start()
    {
        // 특정 방향 바라보게하기
        //Vector3 direction = targetTransform.position - transform.position;
        //Quaternion targetRotation = Quaternion.LookRotation(direction);
        //transform.rotation = targetRotation;

        // 회전, 보간
        //Quaternion aRotation = Quaternion.Euler(new Vector3(30, 0, 0));
        //Quaternion bRotation = Quaternion.Euler(new Vector3(60, 0, 0));
        //Quaternion targetRotation = Quaternion.Lerp(aRotation, bRotation, 0.5f);
        //transform.rotation = targetRotation;

        //Quaternion originalRotate = transform.rotation;
        //Vector3 originalRotationInVector3 = originalRotate.eulerAngles;
        //Vector3 targetRotationInVector3 = originalRotationInVector3 + new Vector3(30, 0, 0);
        //Quaternion targetRotation = Quaternion.Euler(targetRotationInVector3);
        //transform.rotation = targetRotation;

        //Quaternion originalRotation = Quaternion.Euler(new Vector3(45, 0, 0));
        //Quaternion plusRotation = Quaternion.Euler(new Vector3(30, 0, 0));
        //Quaternion targetRotation = originalRotation * plusRotation;
        //transform.rotation = targetRotation;

    }
    // Update is called once per frame
    void Update()
    {
        if( Input.GetKey(KeyCode.Space))
        {
            Move();
        }
    }

    void Move()
    {

        transform.Translate( move * Time.deltaTime/*, Space.World*/ );
    }
}
