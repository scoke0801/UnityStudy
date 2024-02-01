using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// �� �� ����� ���۵Ǹ� �� ���� �� �� �ִ� ���������� �� �� �ִ� �Ѿ��� ���� �Ǵ�.
    /// </summary>

    [CreateAssetMenu(menuName = "PluggableAI/Decisions/End Burst")]
    public class EndBurstDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            // ����� ���� ������ ������.. TRUE( Wait )
            return controller.variables.currentShoots >= controller.variables.shotsinRounds;
        }
    }
}