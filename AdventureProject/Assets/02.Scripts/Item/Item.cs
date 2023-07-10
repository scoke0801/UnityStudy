using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Item
{
    [System.Serializable]
    public class Option
    {
        public ItemInfo info;
        public int amount;
        public int slotIndex;
    }

    private Option _option;

    #region Properties
    public ItemInfo Info { get { return _option.info; } set { _option.info = value; } }
    public int Amount 
    {
        get { return _option.amount; }
        set 
        {
            _option.amount = value;
        }
    }

    public int SlotIndex { get { return _option.slotIndex; } set { _option.slotIndex = value; } }

    public int Index { get { return _option.info.Index; } }
    public Sprite Itemicon { get { return _option.info.ItemIcon; } }
    #endregion

    #region Public Methods
    public Item()
    {
        _option = new Option();
    }
    #endregion
}
