using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// ��� �����ߴ°�?
    /// �÷��װ� ���� ���·� �Ǵ�?
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Feel Alert")]
    public class FeelAlertDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            return controller.variables.feelAlert;
        }
    }
}