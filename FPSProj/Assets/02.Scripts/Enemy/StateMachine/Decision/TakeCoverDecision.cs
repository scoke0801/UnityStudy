using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// ��ֹ��� �̵��� �� �ִ� ��Ȳ���� �ƴ� �� �Ǵ�.
    /// ������ �Ѿ��� �����ְų�, ���󹰷� �̵��ϱ� ���� ��� �ð��� �����ְų�.
    /// ���࿡ �������� ������ ���� ���� ����.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Decisions/Take Cover")]
    public class TakeCoverDecision : Decision
    {
        public override bool Decide(StateController controller)
        {
            // ���� ������ �Ѿ��� ���� �ְų� ,���ð��� �� �ʿ��ϰų�, ���� ��ġ�� ã�����Ͽ��ٸ� false
            if(controller.variables.currentShoots< controller.variables.shotsinRounds ||
                controller.variables.watiInCoverTime > controller.variables.coverTime ||
                Equals(controller.CoverSpot, Vector3.positiveInfinity))
            {
                return false;
            }

            return true;
        }
    }
}