using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonCtrl : MonoBehaviour
{
    GameObject turretObject;
    GameObject missileGizmosObject;
    
    Transform position;
    Transform missilePosition;

    public GameObject missile;
    bool shootOk = true;

    // Start is called before the first frame update
    void Start()
    {
        turretObject = GameObject.Find("Turret");
       //position = turretObject.GetComponent<Transform>();
        position = turretObject.transform;

        missileGizmosObject = GameObject.Find("MissileGizmos");
        missilePosition = missileGizmosObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(position.transform.position, position.transform.forward * 12, Color.red);

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.forward * 3 * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.back * 3 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.right * 3 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.left * 3 * Time.deltaTime);
        }

        if (shootOk)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                StartCoroutine("ShootMissile");
            }
        }

        RaycastHit hit;
        if (Input.GetKey(KeyCode.Space))
        {
            if(Physics.Raycast(position.transform.position, position.transform.forward, out hit, 12.0f))
            {
                Debug.Log(hit.collider.gameObject.name);
            }
            else
            {
                Debug.Log("Nothing");
            }
        }
    }

    IEnumerator ShootMissile()
    {
        shootOk = false;
        missilePosition = missileGizmosObject.transform;
        GameObject newObject = Instantiate(missile, missilePosition.position, missilePosition.transform.rotation);

        Rigidbody rb = newObject.GetComponent<Rigidbody>();
        rb.AddForce(position.transform.forward * 500);

        yield return new WaitForSeconds(0.5f);
        shootOk = true;
    }
}
