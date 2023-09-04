using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(fileName = "ItemDatabase_", menuName = "Inventory System/Items/Database")]
    public class ItemObjectDatabase : ScriptableObject
    {
        public int baseId;
        public ItemObject[] itemObjects;

        public void OnValidate()
        {
            for(int i =0; i < itemObjects.Length; ++i)
            {
                itemObjects[i].data.id = i;
            }
        }
    }
}