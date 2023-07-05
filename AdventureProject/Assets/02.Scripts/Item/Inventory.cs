using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    #region Variables
    private List<Item> _items;
    #endregion

    #region Properties
    public int Index { get; set; }
    #endregion

    public Inventory()
    {
        _items = new List<Item>(Define.Inven.INVEN_MAX);
    }

    public void AddItem(Item item)
    {
        if(_items.Count == _items.Capacity) { return; }

        _items.Add(item);
    }
    public bool SetItem(int index, Item item)
    {
        if( index < 0 || index >= Define.Inven.INVEN_MAX) { return false; }

        _items[index] = item;
        return true;
    }
    public Item GetItem(int index)
    {
        if(index < 0 || index >= _items.Count) { return null; }

        return _items[index];
    }
}
