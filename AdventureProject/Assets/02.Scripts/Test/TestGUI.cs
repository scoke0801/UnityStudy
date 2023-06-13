using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGUI : MonoBehaviour
{
    [SerializeField] ArrowShooter _shooter;

    private void OnGUI()
    {
        if (GUILayout.Button("��", GUILayout.Width(120), GUILayout.Height(30)))
        {
            if (!_shooter) { return; }
        }

        if (GUILayout.Button("�߻�", GUILayout.Width(120), GUILayout.Height(30)))
        {
            if (!_shooter) { return; }
        }
    }
}
