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

            // T�� Quest���, QuestŬ������ ��ӹ޴� Achievement Ŭ������ ���Ե� �� �ֱ⿡ �ѹ� �� Ȯ�����ִ°�
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
