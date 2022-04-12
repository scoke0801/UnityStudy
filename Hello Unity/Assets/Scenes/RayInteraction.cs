using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayInteraction : MonoBehaviour
{
    public LayerMask whatIsTarget;

    public float distance = 100f;

    private Camera playerCam;

    private Transform moveTarget;

    private float targetDistance;
    // Start is called before the first frame update
    void Start()
    {
        playerCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // ī�޶�� ���� �ִ� ȭ����� ���߾��� ������ǥ ���� ��ǥ�� ��ȯ
        Vector3 rayOrigin = playerCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        Vector3 rayDir = playerCam.transform.forward;

        Ray ray = new Ray(rayOrigin, rayDir);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green);

        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, distance, whatIsTarget))
            {
                GameObject hitTarget = hit.collider.gameObject;

                hitTarget.GetComponent<Renderer>().material.color = Color.red;

                Debug.Log(hit.collider.gameObject.name);
                // �浹 ����� ������ �ش� ��Ͽ� ����
                Debug.Log("Detect Someting");

                moveTarget = hitTarget.transform;
                targetDistance = hit.distance; 
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            if (moveTarget != null)
            {
                moveTarget.GetComponent<Renderer>().material.color = Color.white;
            }
            moveTarget = null;
        }

        if (moveTarget != null)
        {
            moveTarget.position = ray.origin + ray.direction * targetDistance;
        }
    }
}
