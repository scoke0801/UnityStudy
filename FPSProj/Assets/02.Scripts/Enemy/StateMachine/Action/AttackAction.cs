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
            throw new System.NotImplementedException();
        }

        private void DoShot(StateController controller, Vector3 direction, Vector3 hitPoint, Vector3 hitNormal= default,
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

            if(target && !organic)
            {
                // 같은 위치에 생성 시 지글지글 하게 보일 수 있으니 조금 거리르 띄워서 배치.
                GameObject bulletHole = EffectManager.Instance.EffectOneShot((int)EffectList.BulletHole,
                    hitPoint + 0.01f * hitNormal);
                bulletHole.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitNormal);

                GameObject instanctSpark = EffectManager.Instance.EffectOneShot((int)EffectList.Sparks, hitPoint);
            }
            else if(target && organic)// player
            {
                HealthBase targetHealth = target.GetComponent<HealthBase>();
                if (targetHealth)
                {
                    targetHealth.TakeDamage(hitPoint, direction, controller.classStats.BulletDamage,
                        target.GetComponent<Collider>(), controller.gameObject);
                }
            }

            SoundManager.Instance.PlayOneShotEffect((int)SoundList.pistol, controller.enemyAnimation.gunMuzzle.position, 2f);
        }
    }

}