using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class QuestSystem : MonoSingleton<QuestSystem>
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

    #region Variables
    public event QuestRegisteredHandler _onQuestRegistered;
    public event QuestCompletedHandler _onQuestCompleted;
    public event QuestCanceledHandler _onQuestCanceled;

    public event QuestRegisteredHandler _onAchievementRegistered;
    public event QuestCompletedHandler _onAchievementCompleted;

    private static bool _isApplicationQuitting = false;

    private List<Quest> _activeQuests = new List<Quest>();
    private List<Quest> _completedQuests = new List<Quest>();

    private List<Quest> _activeAchievements = new List<Quest>();
    private List<Quest> _completedAchievements = new List<Quest>();
    #endregion

    #region Properties
    public IReadOnlyList<Quest> ActiveQuests => _activeQuests;
    public IReadOnlyList<Quest> CompletedQuests => _completedQuests;

    public IReadOnlyList<Quest> ActiveAchievenements => _activeAchievements;

    public IReadOnlyList<Quest> CompletedAchievements => _completedAchievements;
    public bool ContainsInActiveQuests(Quest quest) => _activeQuests.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInCompltedQuest(Quest quest) => _completedQuests.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInActiveAchievement(Quest quest) => _activeAchievements.Any(x => x.CodeName == quest.CodeName);
    public bool ContainsInCompltedAchievement(Quest quest) => _completedAchievements.Any(x => x.CodeName == quest.CodeName);
    #endregion

    #region Unity Events
    private void Awake()
    {
        // TODO
        //_questDatabase = Resources.Load<QuestDatabase>("QuestDatabase");
        //_achievementDatabase = Resources.Load<QuestDatabase>("Achievement_QuestDatabase");

        //if(!Load())
        //{
        //    foreach(var achievements in _achievementDatabase.Quests)
        //    {
        //        Register(achievements);
        //    }
        //}
    }
    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
        // Save();
    }

    #endregion

    #region Public Methods
    // 외부에서 사용할 함수
    public void ReceiveReport(string category, object target, int successCount)
    {
        ReceiveReport(_activeQuests, category, target, successCount);
        ReceiveReport(_activeAchievements, category, target, successCount);
    }

    // 편의를 위한 오버라이딩
    public void ReceiveReport(QuestCategory category, QuestTaskTarget target, int successCount)
    {
        ReceiveReport(category.CodeName, target.Value, successCount);
    }
    public void CompleteWaitngQuests()
    {
        foreach (var quest in ActiveQuests.ToList())
        {
            if (quest.IsComplatable)
            {
                quest.Complete();
            }
        }
    }

    public Quest Register(Quest quest)
    {  
        // ScriptableObject를 Instantiate함수로 복사할 경우, 내부의 Task가 동일하게 복사되어버림
        // 그래서 Quest의 복사본을 만들 때, Quest의 Task들도 복사본으로 바꿔줘야 한다.
        // Quest에서 변경사항이 있을 경우, 해당 함수로 돌아와서 다시 수정... => 복사생성 가능하게 처리하자.
        Quest newQuest = quest.Clone();

        if ( newQuest.GetType() == typeof(Achievement))
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
    #endregion

    #region Private Methods
    // 내부에서 사용하는 함수
    private void ReceiveReport(List<Quest> quests, string category, object target, int successCount)
    {
        // 목록을 순회하는 중에 외부에서 quest 원본이 변경되는 경우 에러가 생길 수 있으니 사본으로.
        foreach (Quest quest in quests.ToArray())
        {
            quest.ReceiveReport(category, target, successCount);
        }
    }
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

        // 다음 프레임에 퀘스트 삭제.
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