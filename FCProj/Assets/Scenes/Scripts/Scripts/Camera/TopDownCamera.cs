using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.VisualScripting;

namespace Cameras
{
    public class TopDownCamera : MonoBehaviour
    {
        #region Variables
        public float _height = 5f;
        public float _distanceFromTarget = 10;
        public float _angleToTarget = 45f;
        public float _lookAtHeight = 2f;

        public float _smoothSpeed = 0.5f;

        private Vector3 _refVelocity;

        public Transform _target;
        #endregion


        private void LateUpdate()
        {
            HandleCamera();
        }

        private void HandleCamera()
        {
            if (!_target) { return; }

            // 카메라의 월드 벡터 계산
            Vector3 worldPos = (Vector3.forward * -_distanceFromTarget) + (Vector3.up * _height);
            Debug.DrawLine(_target.position, worldPos, Color.red);

            // 카메라의 회전값
            Vector3 rotatedVector = Quaternion.AngleAxis(_angleToTarget, Vector3.up) * worldPos;
            Debug.DrawLine(_target.position, rotatedVector, Color.green);

            // 카메라의 위치 설정.
            Vector3 finalTargetPos = _target.position;
            finalTargetPos.y += _lookAtHeight;

            Vector3 finalPosition = finalTargetPos + rotatedVector;
            Debug.DrawLine(_target.position, finalPosition, Color.blue);

            transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref _refVelocity, _smoothSpeed);

            transform.LookAt(transform);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);

            if (_target)
            {
                Vector3 lookAtPosition = _target.position;
                lookAtPosition.y += _lookAtHeight;

                Gizmos.DrawLine(transform.position, lookAtPosition);
                Gizmos.DrawSphere(lookAtPosition, 0.25f);
            }

            Gizmos.DrawSphere(transform.position, 0.25f);
        }
    }
}