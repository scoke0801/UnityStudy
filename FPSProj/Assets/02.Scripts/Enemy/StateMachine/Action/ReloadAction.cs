using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 재장전.
    /// </summary>
    [CreateAssetMenu(menuName = "PluggableAI/Actions/Reload")]
    public class ReloadAction : Action
    { 
        public override void Act(StateController controller)
        {
            // 재장전 중이 아니고, 총알이 남은 경우.
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