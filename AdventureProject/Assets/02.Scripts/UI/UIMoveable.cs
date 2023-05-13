using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMoveable : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private Vector2 _startingPoint;
    private Vector2 _moveBegin;
    private Vector2 _moveOffset;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        _startingPoint = transform.position;
        _moveBegin = eventData.position;
    }
    
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        _moveOffset = eventData.position - _moveBegin;
        transform.position = _startingPoint + _moveOffset;
    }
}
