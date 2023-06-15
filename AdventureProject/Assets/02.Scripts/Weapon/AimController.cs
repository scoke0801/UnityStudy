using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class AimController : MonoBehaviour
{
    [SerializeField] Transform _aimCamera;
    [SerializeField] LayerMask _layerMask;
    Image _aimingUi;

    float _rayMaxDist = 5.0f;

    private void Awake()
    {
        _aimingUi = GetComponent<Image>();
        
        if( !_aimingUi) { return; }
        _aimingUi.enabled = false;

        _layerMask = 0;
    }

    public void StartAim()
    {
        if (!_aimingUi) { return; }

        _aimingUi.enabled = true;
    }

    public void FixedUpdate()
    {
        if (!_aimingUi) { return; }

        
    }

    public void LateUpdate()
    {
        if (!_aimCamera || !_aimingUi) { return; }

        if (Physics.Raycast(_aimCamera.position, _aimCamera.TransformDirection(Vector3.forward), out RaycastHit hit, _rayMaxDist, _layerMask))
        {
            transform.position = hit.point;
        }
        else
        {
            transform.position = _aimCamera.position + _aimCamera.forward * _rayMaxDist;
        }
        transform.LookAt(transform.position + _aimCamera.rotation * Vector3.forward,
            _aimCamera.rotation * Vector3.up);
    }
}
