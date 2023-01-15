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
        // - 플레이어 세이브 로드. ( 직렬화 해서 가능하게 해보자 )
        // - 세이브 정보가 없으면 플레이어를 등록 씩으로 리다이렉션( ?? )
        // - 백엔드를 호출하고 일일 챌린지와 보상을 얻는다. ( 서버? )

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
