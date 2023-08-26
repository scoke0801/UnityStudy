using System.Collections;
using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;

public class AttackBehaviour_Projectile : AttackBehaviour
{
    public override void ExecuteAttack(GameObject target = null, Transform startPoint = null)
    {
        if(target == null)
        {
            return;
        }

        Vector3 projectilePosition = startPoint?.position ?? transform.position;
        if (_effectPrefab)
        {
            GameObject projectilGameObj = GameObject.Instantiate<GameObject>(_effectPrefab, projectilePosition, Quaternion.identity);

            projectilGameObj.transform.forward = transform.forward;

            Projectile projectile = projectilGameObj.GetComponent<Projectile>();

            if (projectile)
            {
                projectile._owner = this.gameObject;
                projectile._target = target;

                projectile._attackBehaviour = this;
            }
        }


        _calcCoolTime = 0.0f;
    }
}
