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

    // Ʈ������ �ݶ��̴��� �浹�� �� �ڵ����� ����
    // Enter..  �浹�� �� ����
    private void OnTriggerEnter(Collider other)
    {
        if ( other.tag == "EndPoint" )
        {
            isOverlapped = true;
            myRenderer.material.color = touchColor; 
        } 
    }
    // Exit.. �浹�ߴٰ� �������� ����
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "EndPoint")
        {
            isOverlapped = false;
            myRenderer.material.color = originalColor; 
        }
    }

    // �浹 �ϰ� �ִ� ����
    private void OnTriggerStay(Collider other)
    {
        
    }

    // �Ϲ� �ݶ��̴��� �浹���� �� �ڵ����� ����
    private void OnCollisionEnter(Collision other)
    {
        
    }
}
