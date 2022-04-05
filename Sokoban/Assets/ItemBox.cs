using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{

    public bool isOverlapped;
    private Renderer myRenderer;
    public Color touchColor;
    private Color originalColor;
    // Start is called before the first frame update
    void Start()
    {
        isOverlapped = false;
        myRenderer = GetComponent<Renderer>();
        originalColor = myRenderer.material.color;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 트리거인 콜라이더와 충돌할 때 자동으로 실행
    // Enter..  충돌을 한 순간
    private void OnTriggerEnter(Collider other)
    {
        if ( other.tag == "EndPoint" )
        {
            isOverlapped = true;
            myRenderer.material.color = touchColor; 
        } 
    }
    // Exit.. 충돌했다가 떼어지는 순간
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "EndPoint")
        {
            isOverlapped = false;
            myRenderer.material.color = originalColor; 
        }
    }

    // 충돌 하고 있는 순간
    private void OnTriggerStay(Collider other)
    {
        
    }

    // 일반 콜라이더와 충돌했을 때 자동으로 실행
    private void OnCollisionEnter(Collision other)
    {
        
    }
}
