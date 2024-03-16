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
            if (GUILayout.Button("Àç»ý"))
            {
                Debug.Log("Àç»ý ¹öÆ° Å¬¸¯");
                SoundManager.Instance.OnPlayButtonClick();
            }
            if (GUILayout.Button("¸ØÃã"))
            {
                Debug.Log("¸ØÃã ¹öÆ° Å¬¸¯!");
                SoundManager.Instance.OnStopButtonClick();
            }
        }
        GUILayout.EndHorizontal();
    
    }

}
