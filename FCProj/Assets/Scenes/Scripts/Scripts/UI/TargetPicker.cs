using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPicker : MonoBehaviour
{
    public float _surfaceOffset = 1.5f;
    public Transform _target = null;

    private void Update()
    {
        if (_target)
        {
            transform.position = _target.position + Vector3.up * _surfaceOffset;
        }
    }

    public void SetPosition(RaycastHit hit)
    {
        _target = null;
        transform.position = hit.point + hit.normal * _surfaceOffset;
    }
}
