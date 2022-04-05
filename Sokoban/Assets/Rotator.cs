using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{   
    // Update is called once per frame
    void Update()
    {
        // Time.deltaTime은 화면이 깜빡이는 시간 = 한 프레임의 시간
        const float ANGLE = 60.0f;
        float deltaAngle = ANGLE * Time.deltaTime;

        transform.Rotate(deltaAngle, deltaAngle, deltaAngle);
    }
}
