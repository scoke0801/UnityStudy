using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPicker : MonoBehaviour
{
    [SerializeField] private float _surfaceOffset = 0.1f;
    [SerializeField] public Transform _target = null;

    public Transform Target { get { return _target; } set { _target = value; } }

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
