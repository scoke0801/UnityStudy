using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectFieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        ObjectFieldOfView fov = (ObjectFieldOfView)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360.0f, fov.ViewRadius);

        Vector3 viewAngleLeft = fov.DirectionFromAngle(-fov.ViewAngle * 0.5f, false);
        Vector3 viewAngleRight = fov.DirectionFromAngle(fov.ViewAngle * 0.5f, false);

        float x = Mathf.Sin(fov.ViewAngle * 0.5f * Mathf.Deg2Rad) * fov.ViewRadius;
        float y = Mathf.Cos(fov.ViewAngle * 0.5f * Mathf.Deg2Rad) * fov.ViewRadius;

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleLeft * fov.ViewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleRight * fov.ViewRadius);

        Handles.color = Color.red;
        foreach(Transform visibleTarget in fov.VisibleTargets)
        {
            Handles.DrawLine(fov.transform.position, visibleTarget.position);
        }
    }

}
