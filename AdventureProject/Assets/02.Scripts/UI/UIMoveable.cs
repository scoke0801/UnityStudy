using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMoveable : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private Transform _targetTransform;

    private Vector2 _startingPoint;
    private Vector2 _moveBegin;
    private Vector2 _moveOffset;

    private void Awake()
    {
        if (!_targetTransform)
        {
            _targetTransform = gameObject.transform;
        }
    }
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        _startingPoint = _targetTransform.position;
        _moveBegin = eventData.position;
    }
    
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        _moveOffset = eventData.position - _moveBegin;
        _targetTransform.position = _startingPoint + _moveOffset;
    }
}
