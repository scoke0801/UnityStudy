using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosTestScript : MonoBehaviour
{
    [SerializeField] Vector3 _pos;
    [SerializeField] Quaternion _rot;

    // Update is called once per frame
    void Update()
    {
        transform.SetPositionAndRotation(_pos, _rot);
    }
}
