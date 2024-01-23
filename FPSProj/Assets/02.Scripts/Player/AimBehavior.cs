using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ���콺 ������ ��ư���� ����. �ٸ� ������ ��ü�ؼ� �����ϰ� �ȴ�.
/// ���콺 �ٹ�ư���� �¿� ī�޶� ����.
/// ���� �𼭸����� ������ �� ��ü�� ��¦ ��￩�ִ� ���.
/// </summary>
public class AimBehavior : GenericBehavior
{
    public Texture2D crossHair; // ���ڼ� �̹���.
    
    public float aimTurnSmoothing = 0.15f; // ī�޶� ���ϵ��� ������ �� ȸ�� �ӵ�,

    public Vector3 aimPivotOffset = new Vector3(0.5f, 1.2f, 0.0f);

    public Vector3 aimCamOffset = new Vector3(0.0f, 0.4f, -0.7f);

    private int aimBool;        // �ִϸ����� �Ķ����, ����
    private bool aim;           // ���� ��?

    private int cornerBool;      // �ִϸ����� �Ķ����, �ڳ�.
    private bool peekCorner;     // �ڳʿ� �ִ� ��?
     
    private Vector3 initialRootRotation;    // ��Ʈ �� ���� ȸ����.
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

    // ī�޶� ���� �÷��̾ �ùٸ� �������� ȸ��.
    void Rotating()
    {
        Vector3 forward = behaviorController.playerCamera.TransformDirection(Vector3.forward);

        forward.y = 0.0f;
        forward = forward.normalized;

        Quaternion targetRotation = Quaternion.Euler(0f, behaviorController.GetCamScript.GetH, 0.0f);

        float minSpeed = Quaternion.Angle(transform.rotation, targetRotation) * aimTurnSmoothing;

        if (peekCorner)
        {
            // ���� ���� �� �÷��̾��� ��ü�� ��¦ ��￩ �ֱ� ����.
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

        // ������ �Ұ����� ������ ���� ���� ����ó��.
        if( behaviorController.GetTempLockStatus(behaviorHashCode) || 
            behaviorController.IsOverriding(this))
        {
            // �̹� �������ΰ��.
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

            // ���� �߿� �̵� ���ϰ�.
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
            // ���� ����.
            StartCoroutine(nameof(ToggleAimOn));
        }
        else if (aim && Input.GetAxisRaw(ButtonName.Aim) == 0)
        {
            // ���� ��
            StartCoroutine(nameof(ToggleAimOff));
        }

        // ���� ���� ���� �޸��� ���� ���ϵ���...
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
