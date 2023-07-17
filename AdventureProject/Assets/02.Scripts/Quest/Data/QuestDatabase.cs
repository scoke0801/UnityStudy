using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

[CreateAssetMenu(menuName = "Quest/QuestDatabase", fileName = "QuestDatabase_")]
public class QuestDatabase : ScriptableObject
{
    #region Variables
    [SerializeField] private List<Quest> _quests;
    #endregion

    #region Properties
    public IReadOnlyList<Quest> Quests => _quests;

    #endregion

    #region Unity Methods
    #endregion

    #region Public Methods
    public Quest FindQuestBy(string codeName)
    {
        for(int i = 0; i < _quests.Count; ++i)
        {
            if (_quests[i].CodeName == codeName)
            {
                return _quests[i];
            }
        }

        return null;
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion

    #region Editor
#if UNITY_EDITOR
    [ContextMenu("FindQuests")]
    private void FindQuests()
    {
        FindQuestBy<Quest>();
    }

    [ContextMenu("FindAchievements")]
    private void FindAchievements()
    {
//        FindQuestBy<Achievement>();
    }

    private void FindQuestBy<T>() where T : Quest
    {
        _quests = new List<Quest>();

        string[] GUIDs = AssetDatabase.FindAssets($"t:{typeof(T)}");

        for(int i = 0; i < GUIDs.Length; ++i)
        {
            string GUID = GUIDs[i];
            string assetPath = AssetDatabase.GUIDToAssetPath(GUID);
            Quest quest = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if(quest.GetType() == typeof(T))
            {
                _quests.Add(quest);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
#endif
    #endregion
}