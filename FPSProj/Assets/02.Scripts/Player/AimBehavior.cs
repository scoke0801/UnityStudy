using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 마우스 오른쪽 버튼으로 조준. 다른 동작을 대체해서 동작하게 된다.
/// 마우스 휠버튼으로 좌우 카메라 변경.
/// 벽의 모서리에서 조준할 때 상체를 살짝 기울여주는 기능.
/// </summary>
public class AimBehavior : GenericBehavior
{
    public Texture2D crossHair; // 십자선 이미지.
    
    public float aimTurnSmoothing = 0.15f; // 카메라를 향하도록 조준할 때 회전 속도,

    public Vector3 aimPivotOffset = new Vector3(0.5f, 1.2f, 0.0f);

    public Vector3 aimCamOffset = new Vector3(0.0f, 0.4f, -0.7f);

    private int aimBool;        // 애니메이터 파라메터, 조준
    private bool aim;           // 조준 중?

    private int cornerBool;      // 애니메이터 파라메터, 코너.
    private bool peekCorner;     // 코너에 있는 지?
     
    private Vector3 initialRootRotation;    // 루트 본 로컬 회전값.
    private Vector3 initialHipRotation;     // 
    private Vector3 initialSpineRotation;   //

    private Transform myTransform;

    private void Start()
    {
        // setup
        aimBool = Animator.StringToHash(Defs.AnimatorKey.Aim);
        cornerBool = Animator.StringToHash(Defs.AnimatorKey.Corner);

        myTransform = transform;

        // Value
        Transform hips = behaviorController.GetAnimator.GetBoneTransform(HumanBodyBones.Hips);
        initialRootRotation = (hips.parent == transform) ? Vector3.zero : hips.parent.localEulerAngles;
        initialHipRotation = hips.localEulerAngles;
        initialSpineRotation = behaviorController.GetAnimator.GetBoneTransform(HumanBodyBones.Spine).localEulerAngles; 
    }

    // 카메라에 따라 플레이어를 올바른 방향으로 회전.
    void Rotating()
    {
        Vector3 forward = behaviorController.playerCamera.TransformDirection(Vector3.forward);

        forward.y = 0.0f;
        forward = forward.normalized;

        Quaternion targetRotation = Quaternion.Euler(0f, behaviorController.GetCamScript.GetH, 0.0f);

        float minSpeed = Quaternion.Angle(transform.rotation, targetRotation) * aimTurnSmoothing;

        if (peekCorner)
        {
            // 조준 중일 때 플레이어의 상체만 살짝 기울여 주기 위함.
            myTransform.rotation = Quaternion.LookRotation(-behaviorController.GetLastDirection());

            targetRotation *= Quaternion.Euler(initialRootRotation);
            targetRotation *= Quaternion.Euler(initialHipRotation);
            targetRotation *= Quaternion.Euler(initialSpineRotation);

            Transform spine = behaviorController.GetAnimator.GetBoneTransform(HumanBodyBones.Spine);

            spine.rotation = targetRotation;
        }
        else
        {
            behaviorController.SetLastDirection(forward);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, minSpeed * Time.deltaTime);
        }
    }

    void AimManagement()
    {
        Rotating();
    }

    private IEnumerator ToggleAimOn()
    {
        yield return new WaitForSeconds(0.05f);

        // 조준이 불가능한 상태일 때에 대한 예외처리.
        if( behaviorController.GetTempLockStatus(behaviorHashCode) || 
            behaviorController.IsOverriding(this))
        {
            // 이미 동작중인경우.
            yield return false;
        }
        else
        {
            aim = true;
            int signal = 1;
            if(peekCorner)
            {
                signal = (int)Mathf.Sign(behaviorController.GetH);
            }
            aimCamOffset.x = Mathf.Abs(aimCamOffset.x) * signal;

            aimPivotOffset.x = Mathf.Abs(aimPivotOffset.x) * signal;
            yield return new WaitForSeconds(0.1f);

            // 조준 중에 이동 못하게.
            behaviorController.GetAnimator.SetFloat(speedFloat, 0.0f);

            behaviorController.OverrideWithBehavior(this);
        }
    }

    private IEnumerator ToggleAimOff()
    {
        aim = false;
        yield return new WaitForSeconds(0.3f);

        behaviorController.GetCamScript.ResetTargetOffsets();

        behaviorController.GetCamScript.ResetMaxVerticalAngle();
        
        yield return new WaitForSeconds(0.1f);

        behaviorController.RevokeOverridingBehavior(this);
    }

    public override void LocalFixedUpdate()
    {
        if (aim)
        {
            behaviorController.GetCamScript.SetTargetOffset(aimPivotOffset, aimCamOffset);
        }
    }

    public override void LocalLateUpdate()
    {
        AimManagement();
    }

    private void Update()
    {
        peekCorner = behaviorController.GetAnimator.GetBool(cornerBool);

        if (Input.GetAxisRaw(ButtonName.Aim) != 0 && !aim)
        {
            // 조준 시작.
            StartCoroutine(nameof(ToggleAimOn));
        }
        else if (aim && Input.GetAxisRaw(ButtonName.Aim) == 0)
        {
            // 조준 끝
            StartCoroutine(nameof(ToggleAimOff));
        }

        // 조준 중일 때는 달리기 하지 못하도록...
        canSprint = !aim;

        if (aim && Input.GetButtonDown(ButtonName.Shoulder) && !peekCorner)
        {
            aimCamOffset.x *= -1;
            aimPivotOffset *= -1;
        }

        behaviorController.GetAnimator.SetBool(aimBool, aim);
    }

    private void OnGUI()
    {
        if(crossHair != null)
        {
            float length = behaviorController.GetCamScript.GetCurrentPivotMagnitude(aimPivotOffset);

            if(length < 0.05f)
            {
                GUI.DrawTexture(new Rect(Screen.width * 0.5f - crossHair.width * 0.5f,
                    Screen.height * 0.5f - crossHair.height * 0.5f,
                    crossHair.width, crossHair.height), crossHair);
            }
        }
    }
}
