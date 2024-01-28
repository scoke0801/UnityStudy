using Defs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class EnemyAnimation : MonoBehaviour
    {
        [HideInInspector] public Animator anim;
        [HideInInspector] public float currentAimingAngleGap;
        [HideInInspector] public Transform gunMuzzle;
        [HideInInspector] public float angularSpeed;

        private StateController controller;
        private NavMeshAgent nav;
        private bool pendingAim;    // ������ ��ٸ��� �ð�. 

        private Transform hips, spine;
        private Vector3 initialRootRotation;
        private Vector3 initialHipsRotation;
        private Vector3 initialSpineRootRotation;

        private Quaternion lastRotation;
        private float timeCountAim, timeCountGuard;
        private readonly float turnSpeed = 25f;     // strafing turn speed.


        private void Awake()
        {
            // setup
            controller = GetComponent<StateController>();
            nav = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            nav.updateRotation = false;

            hips = anim.GetBoneTransform(HumanBodyBones.Hips);
            spine = anim.GetBoneTransform(HumanBodyBones.Spine);

            initialRootRotation = hips.parent == transform ? Vector3.zero : hips.parent.localEulerAngles;

            initialHipsRotation = hips.localEulerAngles;
            initialSpineRootRotation = spine.localEulerAngles;

            anim.SetTrigger(Defs.AnimatorKey.ChangeWeapon);
            anim.SetInteger(Defs.AnimatorKey.Weapon, (int)System.Enum.Parse(typeof(WeaponType), controller.classStats.WeaponType));
        
            foreach(Transform child in anim.GetBoneTransform(HumanBodyBones.RightHand))
            {
                gunMuzzle = child.Find("muzzle");
                if (gunMuzzle != null)
                {
                    break;
                }
            }
            foreach(Rigidbody member in GetComponentsInChildren<Rigidbody>())
            {
                member.isKinematic = true;
            }
        }

        void Setup(float speed, float angle, Vector3 strafeDirection)
        {
            angle *= Mathf.Deg2Rad;
            angularSpeed = angle / controller.generalStats.angleReasponseTime;

            anim.SetFloat(Defs.AnimatorKey.Speed, speed, controller.generalStats.speedDampTime, Time.deltaTime);
            anim.SetFloat(Defs.AnimatorKey.AngularSpeed, angularSpeed, controller.generalStats.angularSpeedDampTime, Time.deltaTime);
            anim.SetFloat(Defs.AnimatorKey.Vertical, strafeDirection.z, controller.generalStats.speedDampTime, Time.deltaTime);

        }

        void NavAnimSetup()
        {
            float speed;
            float angle;

            speed = Vector3.Project(nav.desiredVelocity, transform.forward).magnitude;
            
            // Ư�� ������ ��Ŀ�� �ϰ� ������
            if (controller.focusSight)
            {
                Vector3 dest = (controller.personalTarget - transform.position);
                dest.y = 0.0f;

                angle = Vector3.SignedAngle(transform.forward, dest, transform.up);
                if(controller.Strafing)
                {
                    dest = dest.normalized;
                    Quaternion targetStarfeRotation = Quaternion.LookRotation(dest);
                    transform.rotation = Quaternion.Lerp(transform.rotation,
                        targetStarfeRotation, turnSpeed * Time.deltaTime);
                }
            }
            else
            {
                if(nav.desiredVelocity == Vector3.zero)
                {
                    angle = 0.0f;
                }
                else
                {
                    angle = Vector3.SignedAngle(transform.forward, nav.desiredVelocity, transform.up);
                }
            }

            // �÷��̾ ���Ϸ� �� �� ���� �Ÿ��� �ʵ��� ������ ������ ����.
            if(!controller.Strafing && Mathf.Abs(angle) < controller.generalStats.angleDeadZone)
            {
                transform.LookAt(transform.position + nav.desiredVelocity);
                angle = 0f;
                if(pendingAim && controller.focusSight)
                {
                    controller.Aiming = true;
                    pendingAim = false;
                }
            }

            //Strafe direction
            Vector3 direction = nav.desiredVelocity;
            direction.y = 0.0f;
            direction = direction.normalized;
            direction = Quaternion.Inverse(transform.rotation) * direction;
            Setup(speed, angle, direction);
        }

        private void Update()
        {
            NavAnimSetup();
        }

        private void OnAnimatorMove()
        {
            if (Time.timeScale > 0 && Time.deltaTime > 0)
            {
                // ���� ����.
                nav.velocity = anim.deltaPosition / Time.deltaTime;
                if (!controller.Strafing)
                {
                    transform.rotation = anim.rootRotation;
                }
            }
        }

        private void LateUpdate()
        {
            if (controller.Aiming)
            {
                // ���� �� ȸ�� ��ġ ����.
                Vector3 direction = controller.personalTarget - spine.position;
                if(direction.magnitude < 0.01f || direction.magnitude > 10000000.0f)
                {
                    // �ʹ� ���� ��, �ʹ� ū���� ���� �ʵ���
                    return;
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                targetRotation *= Quaternion.Euler(initialRootRotation);
                targetRotation *= Quaternion.Euler(initialHipsRotation);
                targetRotation *= Quaternion.Euler(initialSpineRootRotation);

                targetRotation *= Quaternion.Euler(VectorHelper.ToVector(controller.classStats.AimOffset));
                Quaternion frameRotation = Quaternion.Slerp(lastRotation, targetRotation, timeCountAim);

                // �����̸� �������� ô�� ȸ���� 60 �������� ���� ��� ������ ����.
                if (Quaternion.Angle(frameRotation, hips.rotation) <= 60.0f)
                {
                    spine.rotation = frameRotation;
                    timeCountAim += Time.deltaTime;
                }
                else
                {
                    // ȸ���� �ʹ� �����ϰ� �������� �ʵ��� ������ ����.
                    if(timeCountAim == 0 && Quaternion.Angle(frameRotation, hips.rotation) > 70.0f)
                    {
                        // 1�� �� ������ Ǯ���ٰ� �ٽ� �״ٰ�.
                        StartCoroutine(controller.UnstuckAim(2f));
                    }

                    spine.rotation = lastRotation;
                    timeCountAim = 0;
                }

                lastRotation = spine.rotation;
                Vector3 target = controller.personalTarget - gunMuzzle.position;
                Vector3 forward = gunMuzzle.forward;
                currentAimingAngleGap = Vector3.Angle(target, forward);

                timeCountGuard = 0;
            }
            else
            {
                lastRotation = spine.rotation;

                // ���� ���� �ƴϸ� ���� offset ������ ���� ���ư�����.
                spine.rotation *= Quaternion.Slerp(Quaternion.Euler(VectorHelper.ToVector(
                    controller.classStats.AimOffset)), Quaternion.identity, timeCountGuard);

                timeCountGuard += Time.deltaTime;
            }
        }

        public void ActivatePendingAim()
        {
            pendingAim = true;
        }

        public void AbortPendingAim()
        {
            pendingAim = false;
            controller.Aiming = false;
        }
    }
}