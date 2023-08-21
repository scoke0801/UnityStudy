using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackBehaviour : AttackBehaviour
{
    public ManualCollision _attackCollision;

    public override void ExecuteAttack(GameObject target = null, Transform startPoint = null)
    {
        Collider[] colliders = _attackCollision?.CheckOverlapBox(_targetMask);

        foreach(Collider collieder in colliders)
        {
            collieder.gameObject.GetComponent<IDamageable>()?.TakeDamage(_damage, _effectPrefab);
        }
    }
}
