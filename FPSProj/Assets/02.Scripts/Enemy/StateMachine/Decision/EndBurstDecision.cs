using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 한 번 사격이 시작되면 한 번에 쏠 수 있는 재장전까지 쏠 수 있는 총알의 수를 판단.
    /// </summary>

    [CreateAssetMenu(menuName = "PluggableAI/Decisions/End Burst")]
    public class EndBurstDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            // 사격을 많이 했으면 재장전.. TRUE( Wait )
            return controller.variables.currentShoots >= controller.variables.shotsinRounds;
        }
    }
}