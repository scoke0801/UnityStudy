using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{ 
    float speed = 10.0f;
    
    public Rigidbody playerRigidBody;

    // 게임이 처음 시작되었을 때 한 번
    void Start()
    {
        playerRigidBody.AddForce(0, 1000, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            playerRigidBody.AddForce(0, 0, speed);
        }
    }
}
