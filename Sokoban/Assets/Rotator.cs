using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{   
    // Update is called once per frame
    void Update()
    {
        // Time.deltaTime�� ȭ���� �����̴� �ð� = �� �������� �ð�
        const float ANGLE = 60.0f;
        float deltaAngle = ANGLE * Time.deltaTime;

        transform.Rotate(deltaAngle, deltaAngle, deltaAngle);
    }
}
