using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 4단계에 걸쳐 사격.
    /// 1. 조준 중이고 조준 유효 각도 안에 타겟이 있거나 가깝다면
    /// 2. 발사 간격 딜레이가 충분히 되었다면 애니메이션을 진행
    /// 3. 충돌 검출을 하는데 약간의 사격 시 충격파도 더해준다.
    /// 4. 총구 이펙트 및 총알 이펙트를 생성.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/Attack")]
    public class AttackAction : Action
    {
        private readonly float startShootDelay = 0.2f;
        private readonly float aimAngleGap = 30f;

        // 초기화
        public override void OnReadyAction(StateController controller)
        {
            controller.variables.shotsinRounds = Random.Range(controller.maximumBurst * 0.5f,
                controller.maximumBurst);

            controller.variables.currentShoots = 0;
            controller.variables.startShootTimer = 0f;
            controller.enemyAnimation.anim.ResetTrigger(Defs.AnimatorKey.Shooting);

            controller.enemyAnimation.anim.SetBool(Defs.AnimatorKey.Crouch, false);

            controller.variables.watiInCoverTime = 0f;

            controller.enemyAnimation.ActivatePendingAim(); // 조준 대기. 해당 시점에서 시야에 들어오면 조준 가능.
        }
        public override void Act(StateController controller)
        {
            controller.focusSight = true;
            if (CanShot(controller))
            {
                Shoot(controller);
            }
            controller.variables.blindEngageTimer += Time.deltaTime;
        }

        private void DoShot(StateController controller, Vector3 direction, Vector3 hitPoint, Vector3 hitNormal = default,
            bool organic = false, Transform target = null)
        {
            GameObject muzzleFlash = EffectManager.Instance.EffectOneShot((int)EffectList.Flash, Vector3.zero);
            muzzleFlash.transform.SetParent(controller.enemyAnimation.gunMuzzle);
            muzzleFlash.transform.localPosition = Vector3.zero;
            muzzleFlash.transform.localEulerAngles = Vector3.left * 90f;

            DestroyDelayed destroyDelayed = muzzleFlash.AddComponent<DestroyDelayed>();
            destroyDelayed.DelayTime = 0.5f;


            GameObject shotTracer = EffectManager.Instance.EffectOneShot((int)EffectList.Tracer, Vector3.zero);
            shotTracer.transform.SetParent(controller.enemyAnimation.gunMuzzle);
            Vector3 origin = controller.enemyAnimation.gunMuzzle.position;
            shotTracer.transform.position = origin;
            shotTracer.transform.rotation = Quaternion.LookRotation(direction);

            if (target && !organic)
            {
                Debug.Log("AttackAction.DoShot1");
                // 같은 위치에 생성 시 지글지글 하게 보일 수 있으니 조금 거리르 띄워서 배치.
                GameObject bulletHole = EffectManager.Instance.EffectOneShot((int)EffectList.BulletHole,
                    hitPoint + 0.01f * hitNormal);
                bulletHole.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitNormal);

                GameObject instanctSpark = EffectManager.Instance.EffectOneShot((int)EffectList.Sparks, hitPoint);
            }
            else if (target && organic)// player
            {
                HealthBase targetHealth = target.GetComponent<HealthBase>();
                Debug.Log("AttackAction.DoShot2");
                if (targetHealth)
                {
                    Debug.Log("AttackAction.DoShot3");
                    targetHealth.TakeDamage(hitPoint, direction, controller.classStats.BulletDamage,
                        target.GetComponent<Collider>(), controller.gameObject);
                }
            }

            SoundManager.Instance.PlayShotSound(controller.classID, controller.enemyAnimation.gunMuzzle.position, 2f);
        }

        private void CastShot(StateController controller)
        {
            // 너무 정사격을 하면 안되니 방향을 틀어줄 용도로..
            Vector3 imprecision = Random.Range(-controller.classStats.ShotErrorRate, controller.classStats.ShotErrorRate) *
                controller.transform.right;

            imprecision += Random.Range(-controller.classStats.ShotErrorRate, controller.classStats.ShotErrorRate) * controller.transform.up;

            Vector3 shotDirection = controller.personalTarget - controller.enemyAnimation.gunMuzzle.position;
            shotDirection = shotDirection.normalized + imprecision;

            Ray ray = new Ray(controller.enemyAnimation.gunMuzzle.position, shotDirection);
            if (Physics.Raycast(ray, out RaycastHit hit, controller.viewRadius, controller.generalStats.shotMask.value))
            {
                Debug.Log("AttackAction.CastShot2");
                bool isOrganic = ((1 << hit.transform.root.gameObject.layer) & controller.generalStats.targetMask) != 0;
                DoShot(controller, ray.direction, hit.point, hit.normal, isOrganic);
            }
            else
            {
                Debug.Log("AttackAction.CastShot2");
                DoShot(controller, ray.direction, ray.origin + (ray.direction * 500f));
            }
        }

        private bool CanShot(StateController controller)
        {
            float distance = (controller.personalTarget - controller.enemyAnimation.gunMuzzle.position).sqrMagnitude;

            // 가까운 거리에 있거나, 조준중이면서 조준 각도안에 들어온경우
            if (controller.Aiming &&
                (controller.enemyAnimation.currentAimingAngleGap < aimAngleGap ||
                distance <= 5.0f))
            {
                // 발사 딜레이가 충분한지 확인.
                if (controller.variables.startShootTimer >= startShootDelay)
                {
                    return true;
                }
                else
                {
                    controller.variables.startShootTimer += Time.deltaTime;
                }
            }
            return false;
        }

        private void Shoot(StateController controller)
        {
            if (Time.timeScale > 0 && controller.variables.shotTimer == 0f)
            {
                Debug.Log("AttackAction.Shoot");
                controller.enemyAnimation.anim.SetTrigger(Defs.AnimatorKey.Shooting);
                CastShot(controller);
            }
            else if (controller.variables.shotTimer >= (0.1f + 2f * Time.deltaTime))
            {
                // 딜레이 타임
                controller.bullets = Mathf.Max(--controller.bullets, 0);
                controller.variables.currentShoots++;
                controller.variables.shotTimer = 0f;
                return;
            }

            controller.variables.shotTimer += controller.classStats.ShotRateFactor * Time.deltaTime;
        }
         
    }

}