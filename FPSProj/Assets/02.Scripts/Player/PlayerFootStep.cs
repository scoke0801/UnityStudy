using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ڱ� �Ҹ���  ���.
/// </summary>
public class PlayerFootStep : MonoBehaviour
{
    public SoundList[] stepSounds;
    private Animator myAnimator;
    private int index;
    private Transform leftFoot, rightFoot;
    private float dist;

    // �ִϸ��̼� ����
    private int groundedBool, coverBool, aimBool, crouchFloat;

    private bool grounded;

    public enum Foot
    {
        LEFT,
        RIGHT,
    }

    private Foot step = Foot.LEFT;
    private float oldDist, maxDist = 0;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        leftFoot = myAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = myAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
        groundedBool = Animator.StringToHash(Defs.AnimatorKey.Grounded);
        coverBool = Animator.StringToHash(Defs.AnimatorKey.Cover);
        aimBool = Animator.StringToHash(Defs.AnimatorKey.Aim);
        crouchFloat = Animator.StringToHash(Defs.AnimatorKey.Crouch);
    }

    private void PlayFootStep()
    {
        if(oldDist < maxDist)
        {
            return;
        }

        oldDist = maxDist = 0;
        int oldIndex = index;
        while(oldIndex == index)
        {
            index = Random.Range(0, stepSounds.Length - 1);
        }

        SoundManager.Instance.PlayOneShotEffect((int)stepSounds[index], transform.position, 0.2f);
    }

    private void Update()
    {
        if(!grounded && myAnimator.GetBool(groundedBool))
        {
            PlayFootStep();
        }
        grounded = myAnimator.GetBool(groundedBool);
        float factor = 0.15f;
        if(grounded && myAnimator.velocity.magnitude > 1.6f)
        {
            oldDist = maxDist;
            switch (step)
            {
                case Foot.LEFT:
                    dist = leftFoot.position.y - transform.position.y;
                    maxDist = dist > maxDist ? dist : maxDist;
                    if(dist <= factor)
                    {
                        PlayFootStep();
                        step = Foot.RIGHT;
                    }
                    break;
                case Foot.RIGHT:
                    dist = rightFoot.position.y - transform.position.y;
                    maxDist = dist > maxDist ? dist : maxDist;
                    if(dist <= factor)
                    {
                        PlayFootStep();
                        step = Foot.LEFT;
                    }
                    break;
            }
        }
    }
}
