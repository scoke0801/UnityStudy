using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertObject : MonoBehaviour
{
    [SerializeField] float _detectRadius = 1.0f;        // 탐지 거리(반지름)
    [SerializeField] float _detectInterval = 0.5f;      // 탐지 간격
    [SerializeField] LayerMask _detectLayerMask;

    public enum AlertState
    {
        Idle,       // 일반상태.
        Concern,    // 주의상태.
        Hostile,    // 적대상태.
    }

    AlertState _alertState;

    float _alertGauge = 0f;

    GameObject _detectedObject;

    private void OnEnable()
    {
        _alertState = AlertState.Idle;
        _alertGauge = 0f;

        _detectedObject = null;

        StartCoroutine(nameof(DetectObjectRoutine));
    }

    public void Alert()
    {
        StopCoroutine(nameof(DetectObjectRoutine));
    }

    private void Update()
    {
        UpdateGauge();

        UpdateState();
    }

    void UpdateGauge()
    {
        if( _detectedObject == null) { return; }

        _alertGauge += Time.deltaTime;
    }

    void UpdateState()
    { 
        if(_alertGauge > 0.75f)
        {
            _alertState = AlertState.Hostile;
        }
        else if(_alertGauge > 0.2f)
        {
            _alertState = AlertState.Concern;
        }
        else
        {
            _alertState = AlertState.Idle;
        }
    }

    IEnumerator DetectObjectRoutine()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(_detectInterval);
        while(_alertGauge < 0.95f)
        {
            DetectObject();

            if (_detectedObject)
            {
                yield return null;
            }

            yield return waitForSeconds;
        }
    }

    void DetectObject()
    {
        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, _detectRadius, _detectLayerMask);

        foreach(Collider coll in targetInViewRadius)
        {
            Debug.Log("Detect Player!");
            _detectedObject = coll.gameObject;
        }
    }

}
