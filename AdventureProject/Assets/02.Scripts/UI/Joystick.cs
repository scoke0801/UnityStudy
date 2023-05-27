using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] GameObject _outCircleImage;
    [SerializeField] GameObject _innerCircleImage;

    Vector2 _touchedPosition;
    float _circleRadius = 0.0f;

    private void Start()
    {
        _outCircleImage.SetActive(false);

        RectTransform rectTransform = _outCircleImage.GetComponent<RectTransform>();
        _circleRadius = rectTransform.sizeDelta.y * 0.5f;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 moveDir = eventData.position - _touchedPosition;
        float distance = Mathf.Min(_circleRadius, moveDir.magnitude);
        _innerCircleImage.transform.position = _touchedPosition + (moveDir.normalized * distance);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _touchedPosition = eventData.position;

        _outCircleImage.SetActive(true);
        _outCircleImage.transform.position = _touchedPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _innerCircleImage.transform.position = _touchedPosition;
        _outCircleImage.SetActive(false);
    }
}
