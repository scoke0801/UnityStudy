using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Observer 
{
    private bool _isTurboOn;
    private Vector3 _initalPosition;
    private float _shakeMagnitude = 0.1f;
    private BikeController _bikeCotroller;

    private void OnEnable()
    {
        _initalPosition = gameObject.transform.localPosition;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isTurboOn)
        {
            gameObject.transform.localPosition =
                _initalPosition + (Random.insideUnitSphere * _shakeMagnitude);
        }
        else
        {
            gameObject.transform.localPosition = _initalPosition;
        }
    }

    public override void Notify(ObserverSubject subject)
    {
        if (!_bikeCotroller)
        {
            _bikeCotroller = subject.GetComponent<BikeController>();
        }
        
        if(_bikeCotroller)
        {
            _isTurboOn = _bikeCotroller.IsTurboOn;
        }
    }
}
