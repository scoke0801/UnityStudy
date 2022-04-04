using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float speed = 10.0f;
    
    private Rigidbody playerRigidBody;

    // 게임이 처음 시작되었을 때 한 번
    void Start()
    {
        // AddForce :: 힘을 주고, 주어진 힘으로 내부에서 속도를 다시 계산
        //playerRigidBody.AddForce(0, 1000, 0);
        playerRigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // -1.0 ~ 1.0
        // A, <- 키 : -1.0
        // 아무것도 안누르면 : 0.0
        // D, -> 키 : 1.0
        // 조이스틱을 고려하여 float값으로 반환
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
         
        float fallSpeed = playerRigidBody.velocity.y;
        Vector3 velocity = new Vector3(inputX, 0.0f, inputZ);
        velocity = velocity * speed;
        velocity.y = fallSpeed;

        playerRigidBody.velocity = velocity;
    }
}
