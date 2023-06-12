using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosTestScript : MonoBehaviour
{
    [SerializeField] Vector3 _pos;
    [SerializeField] Quaternion _rot;
    
    [SerializeField] Vector3 _targetV;
    [SerializeField] Transform _targetT;

    [SerializeField] bool useVector = true;

    // Update is called once per frame
    void Update()
    {
        Quaternion rot = transform.rotation;

        Vector3 target;
        if (useVector)
        {
            target = _targetV;
        }
        else
        {
            target = _targetT.transform.position;
        }

        target.y = transform.position.y;
        transform.LookAt(target);

        transform.Rotate(90, 0, 0);
    }
}
