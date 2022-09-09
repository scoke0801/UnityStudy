using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Target/GameObjectTarget", fileName = "GameObjectTarget")]
public class GameObjectTarget : TaskTarget
{
    [SerializeField]
    private GameObject _value;

    public override object Value => _value;

    public override bool IsEqual(object target)
    {
        GameObject targetAsGameObject = target as GameObject;
        if( targetAsGameObject == null)
        {
            return false;
        }

        return targetAsGameObject.name.Contains(_value.name);
    }
}
