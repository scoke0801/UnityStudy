using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSystem : MonoBehaviour
{
    public Text achivementText;
     
    public void UnLockAchievement(string title)
    {
        Debug.Log("�������� ����! - " + title);
        achivementText.text = "�������� ����:  " + title;
    }
}
