using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void OnPlayerDead()
    {
        // 5초 뒤에 Restart함수를 실행시켜주는 유니티 편의 함수
        Invoke("Restart", 5f);
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
