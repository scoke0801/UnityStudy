using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Category", fileName = "Category_")]
public class Category : ScriptableObject, IEquatable<Category>
{
    [SerializeField]
    private string _displayName;

    [SerializeField]
    private string _codeName;

    public string CodeName => _codeName;
    public string DisplayName => _displayName;

    #region Opeartor
    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override int GetHashCode() => (_displayName, _codeName).GetHashCode();

    public bool Equals(Category other)
    {
        if(other == null)
        {
            return false;
        }

        if( ReferenceEquals(this, other ) )
        {
            return true;
        }

        if( GetType() != other.GetType())
        {
            return false;
        }

        return _codeName == other._codeName;
    }

    public static bool operator==(Category lhs, string rhs)
    {
        if (lhs == null)
        {
            return ReferenceEquals(rhs, null);
        }

        return lhs.CodeName == rhs || lhs.DisplayName == rhs;
    }

    public static bool operator !=(Category lhs, string rhs) => !(lhs == rhs);
    #endregion
}
