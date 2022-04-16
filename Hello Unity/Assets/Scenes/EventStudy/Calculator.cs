using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculator : MonoBehaviour
{
    // ������ �Լ�������
    delegate float Calculate(float a, float b);
    
    // ��������Ʈ Ÿ��
    Calculate onCalulate;

    // Start is called before the first frame update
    void Start()
    {
        onCalulate = Add;
        onCalulate += Sub;
//        onCalulate -= Add;
    }

    public float Add(float a, float b)
    {
        Debug.Log(a + b);
        return a + b;
    }
    public float Sub(float a, float b)
    {
        Debug.Log(a - b);
        return a - b;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            onCalulate(1, 10);
        }
    }
}
