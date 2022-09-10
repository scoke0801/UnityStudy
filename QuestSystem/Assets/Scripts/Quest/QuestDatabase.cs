using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Quest/QuestDatabase", fileName = "QuestDatabase_")]
public class QuestDatabase : ScriptableObject
{
    [SerializeField]
    private List<Quest> _quests;

    public IReadOnlyList<Quest> Quests => _quests;

    public Quest FindQuestBy(string codeName) => _quests.FirstOrDefault(x => x.CodeName == codeName);

#if UNITY_EDITOR

    [ContextMenu("FindQuests")]
    private void FindQuests()
    {
        FindQuestBy<Quest>();
    }

    [ContextMenu("FindAchievements")]
    private void FindAchievements()
    {
        FindQuestBy<Achievement>();
    }

    private void FindQuestBy<T>() where T : Quest
    {
        _quests = new List<Quest>();
         
        string[] GUIDs = AssetDatabase.FindAssets($"t:{typeof(T)}"); 
    
        foreach( string GUID in GUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(GUID);
            Quest quest = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            // T가 Quest라면, Quest클래스를 상속받는 Achievement 클래스가 포함될 수 있기에 한번 더 확인해주는것
            if(quest.GetType() == typeof(T))
            {
                _quests.Add(quest);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
