using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Target/StringTarget", fileName = "StringTarget")]
public class StringTarget : TaskTarget
{
    [SerializeField]
    private string _value;

    public override object Value => _value;

    public override bool IsEqual(object target)
    {
        string targetAsString = target as string;
        if( targetAsString == null)
        {
            return false;
        }

        return _value == targetAsString;
    }
}
