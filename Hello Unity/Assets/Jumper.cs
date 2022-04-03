using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    public Rigidbody myRigidBody;
    // Start is called before the first frame update
    void Start()
    {
        myRigidBody.AddForce(0,500, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
