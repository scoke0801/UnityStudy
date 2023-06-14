using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimController : MonoBehaviour
{
    [SerializeField] Camera _aimCamera;

    Image _aimingUi;
    
    private void Awake()
    {
        _aimingUi = GetComponent<Image>();
        
        if( !_aimingUi) { return; }
        _aimingUi.enabled = false;
    }

    public void StartAim()
    {
        if (!_aimingUi) { return; }

        _aimingUi.enabled = true;
    }

    public void LateUpdate()
    {
        if (!_aimCamera || !_aimingUi) { return; }

        transform.LookAt(transform.position + _aimCamera.transform.rotation * Vector3.forward,
            _aimCamera.transform.rotation * Vector3.up);
    }
}
