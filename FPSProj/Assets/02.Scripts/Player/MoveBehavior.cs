using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �̵��� ���� ������ ����ϴ� ������Ʈ
/// �浹ó���� ���� ��ɵ� ����
/// �⺻ �������ν� �۵�
/// </summary>
public class MoveBehavior : GenericBehavior
{
    public float walkSpeed = 0.15f;
    public float runSpeed = 1.0f;
    public float sprintSpeed = 2.0f;
    public float speedDampTime = 0.1f;

    public float jumpHeight = 1.5f;
    public float jumpInertialForce = 10.0f; //���� ����

    public float speed;
    public float speedSeeker;
    public float jumpSpeed;

    private int jumpBool;
    private int groundedBool;
    private bool isColliding;

    // ĳ��
    private CapsuleCollider capsuleCollider;
    private Transform myTransform;

    private void Awake()
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

}
