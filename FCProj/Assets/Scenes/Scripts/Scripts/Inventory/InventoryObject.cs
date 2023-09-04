using InventorySystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InventorySystem
{
    public enum InterfaceType 
    {
        Inventory,
        Equipment,
        QuickSlot,
        Box,

    }

    [CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
    public class InventoryObject : ScriptableObject
    {
        public ItemObjectDatabase database;
        public InterfaceType type;

        [SerializeField] private Inventory container = new Inventory();

        public InventorySlot[] Slots => container.Slots;

        public int EmptySlotCount
        {
            get
            {
                int counter = 0;
                foreach(InventorySlot slot in Slots)
                {
                    if(slot.item.id < 0)
                    {
                        counter++;
                    }
                }
                return counter;
            }
        }

        public bool AddItem(ItemBase item, int amount)
        {
            if(EmptySlotCount <= 0)
            {
                return false;
            }

            InventorySlot slot = FindItemInInventory(item);
            if (!database.itemObjects[item.id].stackable || slot == null)
            {
                GetEmptySlot().UpdateSlot(item, amount);
            }
            else
            {
                slot.AddAmount(amount);
            }

            return true;
        }

        public InventorySlot FindItemInInventory(ItemBase item)
        {
            return Slots.FirstOrDefault(i => i.item.id == item.id);
        }

        public InventorySlot GetEmptySlot()
        {
            return Slots.FirstOrDefault(i => i.item.id < 0);
        }

        public bool IsContainItem(ItemObject itemObject)
        {
            return Slots.FirstOrDefault(i => i.item.id == itemObject.data.id) != null;
        }

        public void SwapItems(InventorySlot itemSlotA, InventorySlot itemSlotB)
        {
            if (itemSlotA == itemSlotB) { return; }

            if (itemSlotB.CanPlaeceInSlot(itemSlotA.ItemObject) && itemSlotA.CanPlaeceInSlot(itemSlotB.ItemObject))
            {
                InventorySlot tempSlot = new InventorySlot(itemSlotB.item, itemSlotB.amount);
                itemSlotB.UpdateSlot(itemSlotA.item, itemSlotA.amount);
                itemSlotA.UpdateSlot(tempSlot.item, tempSlot.amount);
            }
        }
    }
}