using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class QuestSystem : MonoBehaviour
{
    #region Save Path
    private const string kSaveRootPath = "questSystem";
    private const string kActiveQuestsSavePath = "activeQuests";
    private const string kCompletedQuestsSavePath = "completedQuests";
    private const string kActiveAchievementsSavePath = "activeAchievements";
    private const string kCompletedAchievementsSavePath = "completedAchievements";
    #endregion
    #region Events
    public delegate void QuestRegisteredHandler(Quest newQuest);
    public delegate void QuestCompletedHandler(Quest quest);
    public delegate void QuestCanceledHandler(Quest quest);
    #endregion

    private static QuestSystem _instance;
    private static bool _isApplicationQuitting = false;
    
    public static QuestSystem Instance
    {
        get
        {
            if (!_isApplicationQuitting && _instance == null)
            {
                _instance = FindObjectOfType<QuestSystem>();
                if (_instance == null)
                {
                    _instance = new GameObject("Quest System").AddComponent<QuestSystem>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }
            return _instance;
        }
    }

    private List<Quest> _activeQuests = new List<Quest>();
    private List<Quest> _completedQuests = new List<Quest>();

    private List<Quest> _activeAchievements = new List<Quest>();
    private List<Quest> _completedAchievements = new List<Quest>();

    private QuestDatabase _questDatabase;
    private QuestDatabase _achievementDatabase;

    public event QuestRegisteredHandler _onQuestRegistered;
    public event QuestCompletedHandler _onQuestCompleted;
    public event QuestCanceledHandler _onQuestCanceled;

    public event QuestRegisteredHandler _onAchievementRegistered;
    public event QuestCompletedHandler _onAchievementCompleted; 

    public IReadOnlyList<Quest> ActiveQuests => _activeQuests;
    public IReadOnlyList<Quest> CompletedQuests => _completedQuests;
    public IReadOnlyList<Quest> ActiveAchievements => _activeAchievements;
    public IReadOnlyList<Quest> CompletedAchievements => _activeAchievements;

    private void Awake()
    {
        _questDatabase = Resources.Load<QuestDatabase>("QuestDatabase");
        _achievementDatabase = Resources.Load<QuestDatabase>("Achievement_QuestDatabase");

        if(!Load())
        {
            foreach(var achievements in _achievementDatabase.Quests)
            {
                Register(achievements);
            }
        }
    }

    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
        // Save();
    }

    // �ܺο��� ����� �Լ�
    public void ReceiveReport(string category, object target, int successCount)
    {
        ReceiveReport(_activeQuests, category, target, successCount);
        ReceiveReport(_activeAchievements, category, target, successCount);
    }

    // ���Ǹ� ���� �������̵�
    public void ReceiveReport(Category category, TaskTarget target, int successCount)
    {
        ReceiveReport(category.CodeName, target.Value, successCount);
    }


    // ���ο��� ����ϴ� �Լ�
    private void ReceiveReport(List<Quest> quests, string category, object target, int successCount)
    {
        // ����� ��ȸ�ϴ� �߿� �ܺο��� quest ������ ����Ǵ� ��� ������ ���� �� ������ �纻����.
        foreach(Quest quest in quests.ToArray())
        {
            quest.ReceiveReport(category, target, successCount);
        }
    }

    public void CompleteWaitngQuests()
    { 
        foreach(var quest in ActiveQuests.ToList())
        {
            if (quest.IsComplatable)
            {
                quest.Complete();
            }
        }
    }
    public Quest Register(Quest quest)
    {
        // ScriptableObject�� Instantiate�Լ��� ������ ���, ������ Task�� �����ϰ� ����Ǿ����
        // �׷��� Quest�� ���纻�� ���� ��, Quest�� Task�鵵 ���纻���� �ٲ���� �Ѵ�.
        // Quest���� ��������� ���� ���, �ش� �Լ��� ���ƿͼ� �ٽ� ����... => ������� �����ϰ� ó������.
        Quest newQuest = quest.Clone();

        if (newQuest is Achievement)
        {
            newQuest._onCompleted += OnAchievementCompleted;

            _activeAchievements.Add(newQuest);

            newQuest.OnRegister();
            _onAchievementRegistered?.Invoke(newQuest);
        }
        else // newQuest is Quest
        {
            newQuest._onCompleted += OnQuestCompleted;
            newQuest._onCanceled += OnQuestCanceled;

            _activeQuests.Add(newQuest);

            newQuest.OnRegister();
            _onQuestRegistered?.Invoke(newQuest);
        }
        return newQuest;
    }

    public bool ContainsInActiveQuests(Quest quest) => _activeQuests.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInCompltedQuest(Quest quest) => _completedQuests.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInActiveAchievement(Quest quest) => _activeAchievements.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInCompltedAchievement(Quest quest) => _completedAchievements.Any(x => x.CodeName == quest.CodeName);
  
    public void Save()
    {
        var root = new JObject();
        root.Add(kActiveQuestsSavePath, CreateSaveDatas(_activeQuests));
        root.Add(kCompletedQuestsSavePath, CreateSaveDatas(_completedQuests));
        root.Add(kActiveAchievementsSavePath, CreateSaveDatas(_activeAchievements));
        root.Add(kCompletedAchievementsSavePath, CreateSaveDatas(_completedAchievements));

        PlayerPrefs.SetString(kSaveRootPath, root.ToString());
        PlayerPrefs.Save();
    }

    public bool Load()
    {
        if(PlayerPrefs.HasKey(kSaveRootPath))
        {
            var root = JObject.Parse(PlayerPrefs.GetString(kSaveRootPath));

            LoadSaveDats(root[kActiveQuestsSavePath], _questDatabase, LoadActiveQuest);
            LoadSaveDats(root[kCompletedQuestsSavePath], _questDatabase, LoadCompltedQuest);
            LoadSaveDats(root[kActiveAchievementsSavePath], _questDatabase, LoadActiveQuest);
            LoadSaveDats(root[kCompletedAchievementsSavePath], _questDatabase, LoadCompltedQuest);

            return true;
        }
        return false;
    }
    private JArray CreateSaveDatas(IReadOnlyList<Quest> quests)
    {
        var saveData = new JArray();
        foreach (var quest in quests)
        {
            if (quest.IsSavable)
            {
                saveData.Add(JObject.FromObject(quest.ToSaveData()));
            }
        }
        return saveData;
    }

    private void LoadSaveDats(JToken datasToken, QuestDatabase database, System.Action<QuestSaveData, Quest> onSuccess)
    {
        var datas = datasToken as JArray;
        foreach(var data in datas)
        {
            var saveData = data.ToObject<QuestSaveData>();
            var quest = database.FindQuestBy(saveData.codeName);
            onSuccess.Invoke(saveData, quest);
        }
    }
    
    private void LoadActiveQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = Register(quest);
        newQuest.LoadFrom(saveData);
    }

    private void LoadCompltedQuest(QuestSaveData saveData, Quest quest)
    {
        var newQuest = quest.Clone();
        newQuest.LoadFrom(saveData);

        if (newQuest is Achievement)
        {
            _completedAchievements.Add(newQuest);
        }
        else
        {
            _completedQuests.Add(newQuest);
        }
    }

    #region Callback
    private void OnQuestCompleted(Quest quest)
    {
        _activeQuests.Remove(quest);
        _completedQuests.Add(quest);

        _onQuestCompleted?.Invoke(quest);
    }

    private void OnQuestCanceled(Quest quest)
    {
        _activeQuests.Remove(quest);

        _onQuestCanceled?.Invoke(quest);

        // ���� �����ӿ� ����Ʈ ����.
        Destroy(quest, Time.deltaTime);
    }

    private void OnAchievementCompleted(Quest achievement)
    {
        _activeAchievements.Remove(achievement);
        _completedAchievements.Add(achievement);

        _onAchievementCompleted?.Invoke(achievement);
    }

    #endregion

}
