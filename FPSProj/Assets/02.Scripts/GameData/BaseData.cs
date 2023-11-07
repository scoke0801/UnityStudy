using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ...
/// </summary>
public class BaseData : ScriptableObject
{
    public const string dataDirectory = "/09.ResourceData/Resources/Data/";
    public string[] names = null;

    public BaseData()
    { }

    public int GetDataCount()
    {
        int refValue = 0;
        if (this.names != null)
        {
            refValue = this.names.Length;
        }

        return refValue;
    }

    public string[] GetNameList(bool showId, string filterWord = "")
    {
        string[] retList = new string[0];

        if(this.names == null)
        {
            return retList;
        }

        retList = new string[this.names.Length];
        
        for(int i =0; i < this.names.Length; ++i)
        {
            if(filterWord != "")
            {
                if (names[i].ToLower().Contains(filterWord.ToLower()) == false)
                {
                    continue;
                }
            }

            if(showId)
            {
                retList[i] = i.ToString() + " : " + this.names[i];
            }
            else
            {
                retList[i] = this.names[i];
            }
        }
        return retList;
    }

    public virtual int AddData(string newName)
    {
        return GetDataCount();
    }

    public virtual void RemoveData(int index)
    {

    }

    public virtual void Copy(int index)
    {

    }
}
