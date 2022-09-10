using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Condition", fileName ="Condition_")]
public abstract class Condition : ScriptableObject
{
    [SerializeField]
    private string _description;

    public string Description => _description;

    public abstract bool IsPass(Quest quest);
}
