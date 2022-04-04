using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float speed = 10.0f;
    
    private Rigidbody playerRigidBody;

    // ������ ó�� ���۵Ǿ��� �� �� ��
    void Start()
    {
        // AddForce :: ���� �ְ�, �־��� ������ ���ο��� �ӵ��� �ٽ� ���
        //playerRigidBody.AddForce(0, 1000, 0);
        playerRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // -1.0 ~ 1.0
        // A, <- Ű : -1.0
        // �ƹ��͵� �ȴ����� : 0.0
        // D, -> Ű : 1.0
        // ���̽�ƽ�� ����Ͽ� float������ ��ȯ
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
         
        float fallSpeed = playerRigidBody.velocity.y;
        Vector3 velocity = new Vector3(inputX, 0.0f, inputZ);
        velocity = velocity * speed;
        velocity.y = fallSpeed;

        playerRigidBody.velocity = velocity;
    }
}
