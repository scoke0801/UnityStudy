using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이동과 점프 동작을 담당하는 컴포넌트
/// 충돌처리에 대한 기능도 포함
/// 기본 동작으로써 작동
/// </summary>
public class MoveBehavior : GenericBehavior
{
    public float walkSpeed = 0.15f;
    public float runSpeed = 1.0f;
    public float sprintSpeed = 2.0f;
    public float speedDampTime = 0.1f;

    public float jumpHeight = 1.5f;
    public float jumpInertialForce = 10.0f; //점프 관성

    public float speed;
    public float speedSeeker;
    public float jumpSpeed;

    private bool jump;      // 점프가 가능한 상태인지.
    private int jumpBool;    // 애니메이션 점프 플레그.
    private int groundedBool;
    private bool isColliding;

    // 캐싱
    private CapsuleCollider capsuleCollider;
    private Transform myTransform;

    private void Start()
    {
        myTransform = transform;
        capsuleCollider = GetComponent<CapsuleCollider>();

        jumpBool = Animator.StringToHash(Defs.AnimatorKey.Jump);
        groundedBool = Animator.StringToHash(Defs.AnimatorKey.Jump);
        behaviorController.GetAnimator.SetBool(groundedBool, true);

        behaviorController.SubscribeBehavior(this);
        behaviorController.RegisterDefaultBehavior(this.behaviorHashCode);

        speedSeeker = runSpeed;

    }

    Vector3 Rotating(float horizontal, float vertical)
    {
        Vector3 forward = behaviorController.playerCamera.TransformDirection(Vector3.forward);

        forward.y = 0.0f;
        forward = forward.normalized;

        // forward와 직각을 이루는 벡터
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
        Vector3 targetDirection = Vector3.zero;
        targetDirection = forward * vertical + right * horizontal;

        if(behaviorController.IsMoving() && targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            Quaternion newRotation = Quaternion.Slerp(behaviorController.GetRigidbody.rotation,
                targetRotation, behaviorController.turnSmoothing);

            behaviorController.GetRigidbody.MoveRotation(newRotation);
            behaviorController.SetLastDirection(targetDirection);
        }

        if(!(Mathf.Abs(horizontal) > 0.9f || Mathf.Abs(vertical) > 0.9f))
        {
            behaviorController.Repositioning();
        }

        return targetDirection;
    }

    private void RemoveVertialVelocity()
    {
        Vector3 horizontalVelocity = behaviorController.GetRigidbody.velocity;
        horizontalVelocity.y = 0.0f;
        behaviorController.GetRigidbody.velocity = horizontalVelocity;
    }


    void MovementManagement(float horizontal, float vertical)
    {
        if(behaviorController.IsGrounded())
        {
            behaviorController.GetRigidbody.useGravity = true;
        }
        else if(!behaviorController.GetAnimator.GetBool(jumpBool) &&
            behaviorController.GetRigidbody.velocity.y > 0 )
        {
            // 점프 중이 아닌데, y값이 0보다 크면 어딘가에 껴있는 이상한 상태...
            RemoveVertialVelocity();
        }

        Rotating(horizontal, vertical);
        Vector2 dir = new Vector2(horizontal, vertical);
        speed = Vector2.ClampMagnitude(dir, 1f).magnitude;
        speedSeeker += Input.GetAxis("Mouse ScrollWheel");
        speedSeeker = Mathf.Clamp(speedSeeker, walkSpeed, runSpeed);
        speed *= speedSeeker;
        if(behaviorController.IsSprinting())
        {
            speed = sprintSpeed;
        }

        behaviorController.GetAnimator.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);
    }

    private void OnCollisionStay(Collision collision)
    {
        // 충돌 중.
        isColliding = true;
        if(behaviorController.IsCurrentBehavior(GetBehaviorCode) &&
            collision.GetContact(0).normal.y <= 0.1f)
        {
            float vel = behaviorController.GetAnimator.velocity.magnitude;
            Vector3 targetMove = Vector3.ProjectOnPlane(myTransform.forward,
                collision.GetContact(0).normal).normalized * vel;

            behaviorController.GetRigidbody.AddForce(targetMove, ForceMode.VelocityChange);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }

    void JumpManagement()
    {
        if(jump && !behaviorController.GetAnimator.GetBool(jumpBool) &&
            behaviorController.IsGrounded())
        {
            // 점프가 시작하는 순간.

            // 점프 중에는 이동 못하도록.
            behaviorController.LockTempBehavior(behaviorHashCode);
            behaviorController.GetAnimator.SetBool(jumpBool, true);

            // 점프 중에 마찰을 없애줌
            if( behaviorController.GetAnimator.GetFloat(speedFloat)> 0.1f)
            {
                capsuleCollider.material.dynamicFriction = 0.0f;
                capsuleCollider.material.staticFriction = 0.0f;
                RemoveVertialVelocity();
                float velocity = 2f * Mathf.Abs(Physics.gravity.y) * jumpHeight;
                velocity = Mathf.Sqrt(velocity);
                behaviorController.GetRigidbody.AddForce(Vector3.up * velocity,
                    ForceMode.VelocityChange);
            }
        }
        else if(behaviorController.GetAnimator.GetBool(jumpBool))
        {
            // 점프 중일 때.
            if(!behaviorController.IsGrounded() && !isColliding &&
                behaviorController.GetTempLockStatus())
            {
                behaviorController.GetRigidbody.AddForce(
                    myTransform.forward * jumpInertialForce *
                    Physics.gravity.magnitude * sprintSpeed, ForceMode.Acceleration);
            }

            // 땅에 내려온 순간.
            if( behaviorController.GetRigidbody.velocity.y < 0f &&
                behaviorController.IsGrounded())
            {
                behaviorController.GetAnimator.SetBool(groundedBool, true);
                capsuleCollider.material.dynamicFriction = 0.6f;
                capsuleCollider.material.staticFriction = 0.6f;
                jump = false;
                behaviorController.GetAnimator.SetBool(jumpBool, false);
                behaviorController.UnLockTempBehavior(behaviorHashCode);
            }
        }
    }

    private void Update()
    {
        if( !jump && Input.GetButtonDown(ButtonName.Jump) && 
            behaviorController.IsCurrentBehavior(behaviorHashCode) &&
            !behaviorController.IsOverriding())
        {
            jump = true;
        }
    }

    public override void LocalFixedUpdate()
    {
        MovementManagement(behaviorController.GetH, behaviorController.GetV);
        JumpManagement();
    } 

}
