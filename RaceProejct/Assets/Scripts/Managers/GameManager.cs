using JH.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private DateTime _gameStartTime;
    private DateTime _gameEndTime;

    // Start is called before the first frame update
    void Start()
    {
        // TOOD
        // - �÷��̾� ���̺� �ε�. ( ����ȭ �ؼ� �����ϰ� �غ��� )
        // - ���̺� ������ ������ �÷��̾ ��� ������ �����̷���( ?? )
        // - �鿣�带 ȣ���ϰ� ���� ç������ ������ ��´�. ( ����? )

        _gameStartTime = DateTime.Now;
        DebugWrapper.Log("GameStart @: " + DateTime.Now);
    }

    void OnApplicationQuit()
    {
        _gameEndTime = DateTime.Now;

        TimeSpan timeElapsed = _gameEndTime.Subtract(_gameStartTime);

        DebugWrapper.Log("Game Ended @: " + DateTime.Now);
        DebugWrapper.Log("Game lasted " + timeElapsed);
    }

    private void OnGUI()
    {
        if( GUILayout.Button("Next Scene"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
