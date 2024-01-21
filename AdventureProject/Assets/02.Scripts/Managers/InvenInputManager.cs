using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InvenInputManager : MonoBehaviour
{
    private List<RaycastResult> _raycastResults;
    private PointerEventData _pointerEventData;
    private GraphicRaycaster _graphicRaycaster;

    private void Awake()
    {
        TryGetComponent<GraphicRaycaster>(out _graphicRaycaster);
        if(_graphicRaycaster == null)
        {
            gameObject.AddComponent<GraphicRaycaster>();
        }

        _pointerEventData = new PointerEventData(EventSystem.current);
        _raycastResults = new List<RaycastResult>();
    }

    public void Update()
    {
        _pointerEventData.position = Input.mousePosition;

        CheckButtonDown();
    }

    #region Private Methods
    private T RaycastAndGetFirstComponent<T>() where T : Component
    {
        _raycastResults.Clear();

        _graphicRaycaster.Raycast(_pointerEventData, _raycastResults);
        if(_raycastResults.Count == 0) { return null; }

        foreach(RaycastResult result in _raycastResults)
        {
            T component = result.gameObject.GetComponent<T>();

            if (component) { return component; }
        }

        return null;
    }
    private void CheckButtonDown()
    {
        if (!Input.GetMouseButtonDown(Define.Input.LEFT_CLICK)) { return; }
         
    }
    #endregion

}
