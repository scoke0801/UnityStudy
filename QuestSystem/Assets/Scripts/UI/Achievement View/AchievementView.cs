using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementView : MonoBehaviour
{
    [SerializeField]
    private RectTransform _achievementGroup;

    [SerializeField]
    private AchievementDetailView _achievementDetailViewPrefab;

    private void Start()
    {
        var questSystem = QuestSystem.Instance;

        CreateDetailView(questSystem.ActiveAchievements);
        CreateDetailView(questSystem.CompletedAchievements);

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }
    private void CreateDetailView(IReadOnlyList<Quest> achievements)
    {
        foreach (var achievement in achievements)
        {
            Instantiate(_achievementDetailViewPrefab, _achievementGroup).Setup(achievement);
        }
    }
}
