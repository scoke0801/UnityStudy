using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFacing : MonoBehaviour
{
    Camera _referenceCamera;    // 바라볼 대상 카메라
    public bool _reverseFace;

    public enum Axis
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        back
    }

    public Axis _axis = Axis.Up;
    public Vector3 GetAxis(Axis refAxis)
    {
        switch (refAxis)
        {
            case Axis.Up: return Vector3.up;
            case Axis.Down: return Vector3.down;
            case Axis.Right: return Vector3.right;
            case Axis.Left: return Vector3.left;
            case Axis.Forward: return Vector3.forward;
            case Axis.back: return Vector3.back;
        }
        return Vector3.up;
    }

    private void Awake()
    {
        if (!_referenceCamera)
        {
            _referenceCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        Vector3 targetPos = transform.position + _referenceCamera.transform.rotation * (_reverseFace ? Vector3.forward : Vector3.up);

        Vector3 targetOrientation = _referenceCamera.transform.rotation * GetAxis(_axis);

        transform.LookAt(targetPos, targetOrientation);
    }
}
