using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerIK : MonoBehaviour
{
    private Animator animator;

    //private PlayerController controller;
    private RigidbodyPlayerController controller;

    public LayerMask groundLayerMask;

    private Vector3 rightFootPosition, leftFootPosition, leftFootIKPosition, rightFootIKPosition;
    private Quaternion leftFootIKRotation, rightFootIKRotation;
    private float lastPelvisPositionY, lastRightFootPositionY, lastLeftFootPositionY;

    [Range(0, 2)]
    [SerializeField] private float heightFromGroundRaycast = 1.14f;

    [Range(0, 2)]
    [SerializeField] private float raycastDownDistance = 1.5f;

    [SerializeField] private float pelvisOffset = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        //controller = GetComponent<PlayerController>();
        controller = GetComponent<RigidbodyPlayerController>();
    }

    private void OnAnimatorIK()
    {
        if (!controller.State.isGrounded) { return; }
        MovePelvisHeight();

        // right foot ik position and rotaion - untilse the pro feature in here
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);

        MoveFeetToIKPoint(AvatarIKGoal.RightFoot, rightFootIKPosition, rightFootIKRotation, ref lastRightFootPositionY);


        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);

        MoveFeetToIKPoint(AvatarIKGoal.LeftFoot, leftFootIKPosition, leftFootIKRotation, ref lastLeftFootPositionY);
    }

    private void FixedUpdate()
    {
        if (!controller.State.isGrounded) { return; }

        AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);       
        AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

        // find and raycast to the ground to find positions
        FeetPositionSolver(rightFootPosition, ref rightFootIKPosition, ref rightFootIKRotation);      
        FeetPositionSolver(leftFootPosition, ref leftFootIKPosition, ref leftFootIKRotation);
    }

    void MoveFeetToIKPoint(AvatarIKGoal foot, Vector3 positionIKHolder, Quaternion rotationIKHolder, ref float lastFootPositionY)
    {
        Vector3 targetIKPosition = animator.GetIKPosition(foot);

        if(positionIKHolder != Vector3.zero)
        {
            targetIKPosition = transform.InverseTransformPoint(targetIKPosition);
            positionIKHolder = transform.InverseTransformPoint(positionIKHolder);

            float yVar = Mathf.Lerp(lastFootPositionY, positionIKHolder.y, 0.5f);
            targetIKPosition.y += yVar;

            lastFootPositionY = yVar;

            targetIKPosition = transform.TransformPoint(targetIKPosition);

            animator.SetIKRotation(foot, rotationIKHolder);
        }

        animator.SetIKPosition(foot, targetIKPosition);
    }

    private void MovePelvisHeight()
    {
        if(rightFootIKPosition == Vector3.zero || leftFootIKPosition == Vector3.zero || lastPelvisPositionY == 0)
        {
            lastPelvisPositionY = animator.bodyPosition.y;
            return;
        }

        float leftOffsetPosition = leftFootIKPosition.y - transform.position.y;
        float rightOffsetPosition = rightFootIKPosition.y - transform.position.y;

        float totalOffset = (leftOffsetPosition < rightOffsetPosition) ? leftOffsetPosition : rightOffsetPosition;

        Vector3 newPelvisPosition = animator.bodyPosition + Vector3.up * totalOffset;

        newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, 0.28f);

        animator.bodyPosition = newPelvisPosition;

        lastPelvisPositionY = animator.bodyPosition.y;
    }

    private void FeetPositionSolver(Vector3  fromSkyPosition, ref Vector3 feetIKPosition, ref Quaternion feetIKRotation)
    {
        RaycastHit feetOutHit;

        if(Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit, raycastDownDistance + heightFromGroundRaycast ))
        {
            feetIKPosition = fromSkyPosition;
            feetIKPosition.y = feetOutHit.point.y + pelvisOffset;
            feetIKRotation = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;

            return;
        }

        feetIKPosition = Vector3.zero;
    }

    private  void AdjustFeetTarget(ref Vector3 feetPosition, HumanBodyBones foot)
    {
        feetPosition = animator.GetBoneTransform(foot).position;
        feetPosition.y = transform.position.y + heightFromGroundRaycast;
    }
}
