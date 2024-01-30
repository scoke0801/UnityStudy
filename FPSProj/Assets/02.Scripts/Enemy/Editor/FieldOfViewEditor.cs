using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Enemy;

[CustomEditor(typeof(StateController))]
public class FieldOfViewEditor : Editor
{
    Vector3 DirFromAngle(Transform transform, float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            // local 에 있는 값을 계산에 포함
            angleInDegrees += transform.eulerAngles.y;
        }
        
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad),
            0f, 
            Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void OnSceneGUI()
    {
        StateController fov = target as StateController;
        if(fov == null || fov.gameObject == null)
        {
            return;
        }

        Handles.color = Color.white;

        // Perecption Area(Circle)
        Handles.DrawWireArc(fov.transform.position,
            Vector3.up,
            Vector3.forward,
            360f,
            fov.perceptionRadius);

        // Near
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360f,
            fov.perceptionRadius * 0.5f);

        // 왼쪽 절반 
        Vector3 viewAngleA = DirFromAngle(fov.transform, -fov.viewAngle / 2, false);

        // 오른쪽 절반
        Vector3 viewAngleB = DirFromAngle(fov.transform, fov.viewAngle / 2, false);

        Handles.DrawWireArc(fov.transform.position, Vector3.up, viewAngleA, fov.viewAngle, fov.viewRadius);
        
        // 위치로부터 왼쪽 절반 각도 방향으로 선을 긋는다.
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
        
        // 위치로부터 오른쪽 절반 각도 방향으로 선을 긋는다.
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);

        Handles.color = Color.yellow;
        if(fov.targetInSight && fov.personalTarget != Vector3.zero)
        {
            // 총구로부터 대상까지 선을 긋는다.
            Handles.DrawLine(fov.enemyAnimation.gunMuzzle.position, fov.personalTarget);
        } 
    }
}
