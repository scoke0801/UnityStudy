using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemInfo_", menuName = "ItemInfo")]
public class ItemInfo : ScriptableObject
{
    [System.Serializable]
    public class Option
    {
        public string name;
        public string desc;
        public int index;

        public Sprite iconSprite;
    }

    public Option option;

    #region Properties
    public Sprite ItemIcon { get {return option.iconSprite; } }
    #endregion

    #region Public Methods
    public int Index { get { return option.index;  } }
    public bool IsStackable() { return true; }
    public bool IsEquipItem() { return false; }
    public bool IsQuestItem() { return false; }
    #endregion
}
