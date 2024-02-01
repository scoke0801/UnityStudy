using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Ÿ���� ���̰ų� ��ó�� ������ ���� ��� �ð��� �ʱ�ȭ�ϰ�
    /// �ݴ�� ������ �ʰų� �־��� �ְų� �ϸ� blindEnageTime��ŭ ��ٸ�����
    /// </summary>
    [CreateAssetMenu(menuName ="PluggableAI/Decisions/Engage")]
    public class EngageDecision : Decision
    {
        [Header("Extra Decision")]
        public LookDecision isViewing;
        public FocusDecision targetNaear;

        public override bool Decide(StateController controller)
        {
            if(isViewing.Decide(controller) || targetNaear.Decide(controller))
            {
                controller.variables.blindEngageTimer = 0;
            }
            else if(controller.variables.blindEngageTimer >= controller.blindEngageTime)
            {
                controller.variables.blindEngageTimer = 0;
                return false;
            }

            return true;
        }
    }
}