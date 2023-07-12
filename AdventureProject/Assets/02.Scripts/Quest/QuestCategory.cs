using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Category", fileName = "QuestCategory_")]
public class QuestCategory : ScriptableObject, IEquatable<QuestCategory>
{
    #region Variables
    [SerializeField]
    private string _displayName;

    [SerializeField]
    private string _codeName;
    #endregion

    #region Properties
    public string CodeName => _codeName;
    public string DisplayName => _displayName;
    #endregion

    #region Unity Methods
    #endregion

    #region Public Methods
    public override bool Equals(object other)
    {
        return base.Equals(other);
    }
    public bool Equals(QuestCategory other)
    {
        if (other == null)
        {
            return false;
        }

        if (ReferenceEquals(other, this))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        return _codeName == other._codeName;
    }
    public override int GetHashCode() => (_codeName, _displayName).GetHashCode();

    public static bool operator==(QuestCategory lhs, string rhs)
    {
        if (lhs is null)
        {
            return ReferenceEquals(rhs, null);
        }

        return lhs.CodeName == rhs || lhs.DisplayName == rhs;
    }

    public static bool operator!=(QuestCategory lhs, string rhs) => !(lhs == rhs);
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion
}
