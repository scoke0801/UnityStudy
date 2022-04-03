using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lecture0402 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("LogTest - from Lecture0402::start()");

        int age = 300;
        int money = -1000;

        Debug.Log(age);
        Debug.Log(money);

        float pi = 3.141592f;

        Debug.Log(pi);

        Debug.Log("내 나이는 " + age);

        Debug.Log("가진 돈은 " + money);

        var myName = "JongHyun";

        Debug.Log("내 이름은 " + myName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
