using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Ÿ���� �ָ� �ְ� ���󹰿��� �ּ� �� Ÿ�������� ������ ��ٸ� �Ŀ� ���� ��ֹ��� �̵�����..
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Advance Cover")]
    public class AdvanceCoverDecision : Decision
    {
        public int waitRounds = 1;

        [Header("Extra Decision")]
        [Tooltip("�÷��̾ ������ �ִ� �� �Ǵ�")]
        public FocusDecision targetNear;

        public override void OnEnableDecision(StateController controller)
        {
            controller.variables.waitRounds += 1;

            // �Ǵ�.
            controller.variables.advanceCoverDecision = Random.Range(0f, 1f) 
                < controller.classStats.ChangeCoverChance / 100f;
        }


        public override bool Decide(StateController controller)
        {
            if(controller.variables.waitRounds <= waitRounds)
            {
                return false;
            }

            controller.variables.waitRounds = 0;
            // Ž���� ��ֹ��� �����ϰ�, Ÿ���� �ʹ� ������ �������.
            // (Ÿ���� ������ �ִµ� ��ֹ��� Ž���ϴ� �ൿ�� ������״�... )
            return controller.variables.advanceCoverDecision && !targetNear.Decide(controller);
        }
    }
}