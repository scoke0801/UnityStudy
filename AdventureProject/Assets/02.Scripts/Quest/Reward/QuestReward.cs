using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Reward", fileName = "Reward_")]
public abstract class QuestReward : ScriptableObject
{
    [SerializeField]
    private Sprite _icon;
    [SerializeField]
    private string _description;
    [SerializeField]
    private int _quantity;

    public Sprite Icon => _icon;
    public string Description => _description;
    public int Quantity => _quantity;

    public abstract void Give(Quest quest);
}
