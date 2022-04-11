using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public UnityEvent onReset;
    
    public GameObject readyPannel;
    public Text scoreText;
    public Text bestScoreText;
    public Text messageText;

    public bool isRoundActive = false;
    private int score = 0;

    public ShooterRotator shooterRotator;
    public CamFollow cam;

    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
        UpdateUI();
    }

    private void Start()
    {
        StartCoroutine("RoundRoutine");
    }
    public void AddScore(int newScore)
    {
        score += newScore;
        UpdateBestScore();
    }

    void UpdateBestScore()
    {
        if (GetBestScore() < score)
        {
            PlayerPrefs.SetInt("BestScore", score);
        }
    }

    int GetBestScore()
    {
        return PlayerPrefs.GetInt("BestScore");
    }

    // Update is called once per frame
    void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        bestScoreText.text = "Best Score: " + GetBestScore();
    }

    public void OnBallDestroy()
    {
        UpdateUI();
        isRoundActive = false;
    }

    public void Reset()
    {
        score = 0;
        UpdateUI();

        // 라운드를 다시 처음부터 시작
        StartCoroutine("RoundRoutine");
    }

    IEnumerator RoundRoutine()
    {
        // READY
        onReset.Invoke();

        readyPannel.SetActive(true);
        cam.SetTarget( shooterRotator.transform, CamFollow.State.Idle );
        shooterRotator.enabled = false;

        isRoundActive = false;

        messageText.text = "Ready...";
        yield return new WaitForSeconds(3f);

        // PLAY
        isRoundActive = true;
        readyPannel.SetActive(false);
        shooterRotator.enabled = true;

        cam.SetTarget(shooterRotator.transform, CamFollow.State.Ready);
        while(isRoundActive)
        {
            // 공이 파괴되는 경우에 무한루프를 탈출 
            yield return null;
        }
        // End

        readyPannel.SetActive(true);
        shooterRotator.enabled = false;

        messageText.text = "Wait For Next Round...";

        yield return new WaitForSeconds(3f);

        Reset();
    }
}
