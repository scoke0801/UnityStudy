using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TempGUIContainer : MonoBehaviour
{
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("���"))
            {
                Debug.Log("��� ��ư Ŭ��");
                SoundManager.Instance.OnPlayButtonClick();
            }
            if (GUILayout.Button("����"))
            {
                Debug.Log("���� ��ư Ŭ��!");
                SoundManager.Instance.OnStopButtonClick();
            }
        }
        GUILayout.EndHorizontal();
    
    }

}
