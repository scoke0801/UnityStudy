using InventorySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Inventory 
{
    private InventorySlot[] slots = new InventorySlot[24];

    public void Clear()
    {
        foreach(InventorySlot slot in slots)
        {
            slot.RemoveItem();
        }
    }

    // 해다 아이템이 포함되어 있는지.
    public bool IsContain(ItemObject itemObject)
    {
        return Array.Find(slots, i => i.item.id == itemObject.data.id) != null;
    }

    public bool IsContain(int id)
    {
        return slots.FirstOrDefault(i => i.item.id == id) != null;
    }
}
