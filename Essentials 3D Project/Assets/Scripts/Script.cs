using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script : MonoBehaviour
{
    [SerializeField]
    private string myMessage;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(myMessage + " start");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(myMessage + " Update");
    }
}
