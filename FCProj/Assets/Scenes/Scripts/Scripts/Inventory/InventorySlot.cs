using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class InventorySlot
    {
        public ItemType[] _allowedItems = new ItemType[0];

        [NonSerialized] public InventroyObject _parent;

        [NonSerialized] public GameObject _slotUI;

        [NonSerialized] public Action<InventorySlot> OnPreUpdate;

        [NonSerialized] public Action<InventorySlot> OnPostUpdate;

        public ItemBase item;
        public int amount;

        public ItemObject ItemObject
        {
            get
            {
                return item.id >= 0 ? _parent.database.itemObjects[item.id] : null;
            }
        }

        public InventorySlot()
        {
            UpdateSlot(new ItemBase(), 0);
        }

        public InventorySlot(ItemBase item, int amoun)
        {
            UpdateSlot(item, amount);
        }

        public void RemoveItem()
        {
            UpdateSlot(new ItemBase(), 0);
        }

        public void AddAmount(int value)
        {
            UpdateSlot(item, amount += value);
        }

        public void UpdateSlot(ItemBase item, int amount)
        {
            OnPreUpdate?.Invoke(this);

            this.item = item;
            this.amount = amount;

            OnPostUpdate?.Invoke(this);
        }

        public bool CanPlaeceInSlot(ItemObject itemObject)
        {
            if (_allowedItems.Length <= 0 || itemObject == null || itemObject.data.id < 0)
            {
                return true;
            }

            foreach (ItemType type in _allowedItems)
            {
                if (itemObject.type == type)
                {
                    return true;
                }
            }
            return false;
        }
    }

}