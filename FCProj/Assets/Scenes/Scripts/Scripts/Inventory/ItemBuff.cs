using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    public enum CharacterAttribute
    {
        Agility,
        Intellect,
        Stamina,
        Strength,

    }

    [Serializable]
    public class ItemBuff
    {
        public CharacterAttribute _stat;
        public int _value;

        [SerializeField] private int _min;
        [SerializeField] private int _max;

        public int Min => _min;
        public int Max => _max;

        public ItemBuff(int min, int max)
        {
            _min = min;
            _max = max;

            GenerateValue();
        }

        public void GenerateValue()
        {
            _value = UnityEngine.Random.Range(_min, _max);
        }

        public void AddValue(ref int value)
        {
            value += _value;
        }
    }

    
}