using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라 속성 중 중요 속성 하나는 카메라로부터 오프셋 벡터, 피봇 오프셋 벡터
/// 위치 오프셋 벡터는 충돌 처리용으로 사용하고 피봇 오프셋 벡터는 시선 이동에 사용하도록
/// 충돌체크 : 이중 충돌 체크 기능( 캐릭터로부터 카메라, 카메라로부터 캐릭터 사이 )
/// recoil( 사격 반동 ) 을 위한 기능
/// FOV 변경 기능.
/// </summary>
public class ThirdPirsonOrbitCamera : MonoBehaviour
{
    public Transform player; // player Transform
    public Vector3 pivotOffset = new Vector3(0.0f, 1.0f, 0.0f);
    public Vector3 camOffset = new Vector3(0.4f, 0.5f, -2.0f);

    public float smooth = 10.0f; // 카메라 반응 속도.
    public float horizontalAimingSpeed = 6.0f; // 수평 회전 속도
    public float verticalAimingSpeed = 6.0f; // 수직 회전 속도
    public float maxVerticalAngle = 30.0f;  //카메라 수직 최대 각도.
    public float minVerticalAngle = -60.0f; // 카메라의 수직 최소 각도

    public float recoilAngleBounce = 5.0f;// 사격 반동 바운스 값.
    private float angleH = 0.0f;    // 마우스 이동에 따른 카메라 수평이동 수치.
    private float angleV = 0.0f;    // 마우스 이동에 따른 카메라 수직이동 수치.

    private Transform cameraTransform; // 트랜스폼 캐싱.
    private Camera myCamera;
    private Vector3 relCameraPos;   // 플레이어로부터 카메라"까지의" 벡터.
    private float relCameraPosMag;  // 플레이어로부터 카메라"사이의"거리.

    private Vector3 smoothPivotOffset;  // 카메라 피봇 보간용 벡터.
    private Vector3 smoothCamOffset;    // 카메라 위치 보간용 벡터.

    private Vector3 targetPivotOffset;  // 카메라 피봇용 보간용 벡터.
    private Vector3 targetCamOffset;    // 카메라 위치 보간용 벡터.

    private float defaultFOV;           // 기본 시야값.
    private float targetFOV;            // 타겟 시야값.
    private float targetMaxVerticalAngle;   // 카메라 수직 최대 각도.
    private float recoilAngle = 0.0f;   // 사격 반동 각도.

    public float GetH
    {
        get
        {
            return angleH;
        }
    }
}
