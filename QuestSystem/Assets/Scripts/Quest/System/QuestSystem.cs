using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
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

        foreach(Quest achievement in _achievementDatabase.Quests)
        {
            Register(achievement);
        }
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
