using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSystem : MonoBehaviour
{
    public Text achivementText;
     
    public void UnLockAchievement(string title)
    {
        Debug.Log("도전과제 해제! - " + title);
        achivementText.text = "도전과제 해제:  " + title;
    }
}
