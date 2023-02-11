using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GizmoForEmptyObject : MonoBehaviour
{
    public Color _color = Color.yellow;
    public float _radius = 0.1f;

    private void OnDrawGizmos()
    {
        Gizmos.color = _color;

        Gizmos.DrawSphere(transform.position, _radius);
    }
}
