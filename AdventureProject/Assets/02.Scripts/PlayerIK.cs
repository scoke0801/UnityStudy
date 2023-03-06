using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerIK : MonoBehaviour
{
    protected Animator animator;

    [Range(0, 1f)]
    public float distanceToGround = 1f;

    public LayerMask groundLayerMask;

    private void Awake()
    {
        animator = GetComponent<Animator>();    
    }
    private void OnAnimatorIK()
    {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);

        Vector3 leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
        Ray leftRay = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);

        if( Physics.Raycast(leftRay, out RaycastHit leftHit, distanceToGround + 1f, groundLayerMask))
        {
            if(leftHit.transform.tag == "Untagged")
            {
                Vector3 footPosition = leftHit.point;
                footPosition.y += distanceToGround;

                animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFoot);
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, leftHit.normal));
            }
        }

        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);

        Vector3 rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot).position;
        Ray rightRay = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);

        if (Physics.Raycast(leftRay, out RaycastHit rightHit, distanceToGround + 1f, groundLayerMask))
        {
            if (rightHit.transform.tag == "Untagged")
            {
                Vector3 footPosition = rightHit.point; 
                footPosition.y += distanceToGround;

                animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFoot);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, rightHit.normal));
            }
        }

        //Vector3 rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot).position;
        //Ray rightRay = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);




        //model.localPosition = new Vector3(0, -Mathf.Abs(leftFoot.y - rightFoot.y) * 0.5f, 0);




        //animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
        //animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
        //animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFoot);
        //animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, rightFoot.normalized));
    }
}
