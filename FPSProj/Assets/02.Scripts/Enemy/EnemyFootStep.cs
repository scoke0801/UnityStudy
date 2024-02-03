using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyFootStep : MonoBehaviour
    {
        public SoundList[] stepSoundLists;

        private int index;

        private Animator anim;

        private bool isLeftFootAhead;

        private bool playedLeftFoot;
        private bool playedRightFoot;

        private Vector3 leftFootIKPos;
        private Vector3 rightFootIKPos;


        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            leftFootIKPos = anim.GetIKPosition(AvatarIKGoal.LeftFoot);
            rightFootIKPos = anim.GetIKPosition(AvatarIKGoal.RightFoot);
        }

        void PlayeFootSetp()
        {
            int oldIndex = index;
            while(oldIndex == index)
            {
                index = Random.Range(0, stepSoundLists.Length);
            }

            SoundManager.Instance.PlayOneShotEffect((int)stepSoundLists[index], transform.position, 1f);
        }

        private void Update()
        {
            float factor = 0.15f;
            if(anim.velocity.magnitude > 1.4f)
            {
                if(Vector3.Distance(leftFootIKPos, anim.pivotPosition) < factor && playedLeftFoot == false)
                {
                    PlayeFootSetp();
                    playedLeftFoot = true;
                    playedRightFoot = false;
                }

                if(Vector3.Distance(rightFootIKPos, anim.pivotPosition) <= factor && playedRightFoot == false)
                {
                    PlayeFootSetp();
                    playedLeftFoot = false;
                    playedRightFoot = true;
                }
            }
        }
    }
}