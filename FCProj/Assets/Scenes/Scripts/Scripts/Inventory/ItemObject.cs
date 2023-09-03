using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InventorySystem
{
    public enum ItemType : int
    {
        Helmet = 0,
        Chest,
        Pants,
        Boots,
        Pauldrons,
        Gloves,
        LeftWeapon,
        RightWeapon,
        Food,
        Default,
    }
    [CreateAssetMenu(fileName = "Item_", menuName = "InventoryStstem/Items/Item")]
    public class ItemObject : ScriptableObject
    {
        public ItemType type;
        public bool stackable;

        public Sprite icon;
        public GameObject modelPrefab;

        public ItemBase data = new ItemBase();

        public List<string> boneNames = new List<string>();

        [TextArea(15, 20)] public string description;

        private void OnValidate()
        {
            boneNames.Clear();

            if(modelPrefab == null || modelPrefab.GetComponentInChildren<SkinnedMeshRenderer>() == null)
            {
                return;
            }

            SkinnedMeshRenderer renderer = modelPrefab.GetComponentInChildren<SkinnedMeshRenderer>();

            Transform[] bones = renderer.bones;

            foreach(Transform t in bones)
            {
                boneNames.Add(t.name);
            }
        }

        public ItemBase CreateItem()
        {
            ItemBase newItem = new ItemBase();

            return newItem;
        }
    }

}