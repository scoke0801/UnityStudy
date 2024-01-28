using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{
    /// <summary>
    /// ���� üũ�ϴ� Ŭ����.
    /// ���� üũ�� �� ��Ư�� ��ġ�κ��� ���ϴ� �˻� �ݰ濡 �ִ� �浹ü�� ã�Ƽ�, �� �ȿ� Ÿ���� �ִ� �� Ȯ��.
    /// </summary>
    public abstract class Decision : ScriptableObject
    {
        public abstract bool Decide(StateController controller);

        public virtual void OnEnableDecision(StateController controller)
        {

        }

        public delegate bool HandleTargets(StateController controller, bool hasTargets, Collider[] targetsInRadius);
    
        public static bool CheckTargetsInradius(StateController controller, float radius, HandleTargets handleTargets)
        {
            if(controller.aimTarget.root.GetComponent<HealthBase>().IsDead)
            {
                return false;
            }
            else
            {
                Collider[] targetsInRadius =
                    Physics.OverlapSphere(controller.transform.position, radius,
                        controller.generalStats.targetMask);
                return handleTargets(controller, targetsInRadius.Length > 0, targetsInRadius);
            }
        }
    }
}