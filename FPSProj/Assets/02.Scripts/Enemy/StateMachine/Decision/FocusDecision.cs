using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{

    /// <summary>
    /// Sense Ÿ�Կ� ���� Ư�� �Ÿ��� ���� ������ ������ �þߴ� ������ �ʾ����� �����Ҹ� �����߰ų�
    /// �ʹ� ����� �Ÿ��� Ÿ��(�÷��̾�)�� �ִ� �� �Ǵ�.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Focus")]
    public class FocusDecision : Decision
    {
        public enum Sense
        {
            NEAR,           // ����
            PERCEPTION,     // ���� ������ �ִ°�
            VIEW,           // ���̴°�
        }

        [Tooltip("� ũ��� ���� ��� ������ ���� ������ Ÿ��.")]
        public Sense sense;

        [Tooltip("���� ������ ������ �� ����")]
        public bool invalidateCoverSpot;

        private float radius;   // sense->range

        public override void OnEnableDecision(StateController controller)
        {
            switch (sense)
            {
                case Sense.NEAR:
                    radius = controller.nearRadius;
                    break;
                case Sense.PERCEPTION:
                    radius = controller.perceptionRadius;
                    break;
                case Sense.VIEW:
                    radius = controller.viewRadius;
                    break;

                default:
                    radius = controller.nearRadius;
                    break;
            }
        }

        private bool MyHandleTargets(StateController controller, bool hasTarget, Collider[] targetsInNearRadius)
        {
            // Ÿ��(�÷��̾�)�� �����ϰ� �þ߰� ������ �ʾҴٸ�
            if (hasTarget && !controller.BlockedSight())
            {
                // 
                if (invalidateCoverSpot)
                {
                    controller.CoverSpot = Vector3.positiveInfinity;
                }

                controller.targetInSight = true;
                controller.personalTarget = controller.aimTarget.position;

                return true;
            }

            return false;
        }

        public override bool Decide(StateController controller)
        {
            // 1.����� �Ÿ��� ���������� �ƴѵ�, �ֺ����� ��� �ް� �þ߰� ������ �ʾҴٸ�.
            // 2. Ÿ���� �Ÿ��ȿ� ���Դٸ�
            return (sense != Sense.NEAR && controller.variables.feelAlert && !controller.BlockedSight()) ||
                Decision.CheckTargetsInradius(controller, radius, MyHandleTargets);
        }
    }
}