using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// ������.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/Reload")]
    public class ReloadAction : Action
    { 
        public override void Act(StateController controller)
        {
            // ������ ���� �ƴϰ�, �Ѿ��� ���� ���.
            if (!controller.reloading && controller.bullets <= 0)
            {
                controller.enemyAnimation.anim.SetTrigger(Defs.AnimatorKey.Reload);
                controller.reloading = true;
                SoundManager.Instance.PlayOneShotEffect((int)SoundList.reloadWeapon,
                    controller.enemyAnimation.gunMuzzle.position, 2f);
            }
        }
    }

}