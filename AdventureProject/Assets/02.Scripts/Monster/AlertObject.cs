using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertObject : MonoBehaviour
{
    [SerializeField] float _detectRadius = 1.0f;        // 탐지 거리(반지름)
    [SerializeField] float _detectInterval = 0.5f;      // 탐지 간격
    [SerializeField] LayerMask _detectLayerMask;
    [SerializeField] Image _alertImage;
    [SerializeField] Canvas _alertCanvas;
    public enum AlertState
    {
        Idle,       // 일반상태.
        Concern,    // 주의상태.
        Hostile,    // 적대상태.
    }

    AlertState _alertState;

    float _alertGauge = 0f;

    GameObject _detectedObject;

    private void Awake()
    {
        _alertCanvas = GetComponentInChildren<Canvas>();
        if(_alertCanvas)
        {
            _alertImage = _alertCanvas.transform.GetChild(0)?.GetComponent<Image>();
        }
    }
    private void OnEnable()
    {
        _alertState = AlertState.Idle;
        _alertGauge = 0f;

        _detectedObject = null;

        if (_alertCanvas)
        {
            _alertCanvas.gameObject.SetActive(false);
        }

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

        if (_alertImage && _alertImage.enabled)
        {
            _alertImage.transform.localPosition = transform.localPosition + new Vector3(0f, 200f, 0f);
        }
    }

    private void LaterUpdate()
    {
        
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
            if (_alertCanvas)
            {
                _alertCanvas.gameObject.SetActive(true);
            }
            _detectedObject = coll.gameObject;
        }
    }

}
