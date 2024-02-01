using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 4�ܰ迡 ���� ���.
    /// 1. ���� ���̰� ���� ��ȿ ���� �ȿ� Ÿ���� �ְų� �����ٸ�
    /// 2. �߻� ���� �����̰� ����� �Ǿ��ٸ� �ִϸ��̼��� ����
    /// 3. �浹 ������ �ϴµ� �ణ�� ��� �� ����ĵ� �����ش�.
    /// 4. �ѱ� ����Ʈ �� �Ѿ� ����Ʈ�� ����.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/Attack")]
    public class AttackAction : Action
    {
        private readonly float startShootDelay = 0.2f;
        private readonly float aimAngleGap = 30f;

        // �ʱ�ȭ
        public override void OnReadyAction(StateController controller)
        {
            controller.variables.shotsinRounds = Random.Range(controller.maximumBurst * 0.5f,
                controller.maximumBurst);

            controller.variables.currentShoots = 0;
            controller.variables.startShootTimer = 0f;
            controller.enemyAnimation.anim.ResetTrigger(Defs.AnimatorKey.Shooting);

            controller.enemyAnimation.anim.SetBool(Defs.AnimatorKey.Crouch, false);

            controller.variables.watiInCoverTime = 0f;

            controller.enemyAnimation.ActivatePendingAim(); // ���� ���. �ش� �������� �þ߿� ������ ���� ����.
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
                // ���� ��ġ�� ���� �� �������� �ϰ� ���� �� ������ ���� �Ÿ��� ����� ��ġ.
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