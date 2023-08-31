using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    [Serializable]
    public class ItemBase
    {
        public int id = -1;
        public string name;

        public ItemBuff[] buffs;

        public ItemBase()
        {
            id = -1;
            name = "";
        }

        public ItemBase(ItemObject itemObject)
        {
            name = itemObject.name;
            id = itemObject.data.id;

            buffs = new ItemBuff[itemObject.data.buffs.Length];
            for (int i = 0; i < buffs.Length; ++i)
            {
                buffs[i] = new ItemBuff(itemObject.data.buffs[i].Min, itemObject.data.buffs[i].Max)
                {
                    _stat = itemObject.data.buffs[i]._stat
                };
            }
        }
    }
}