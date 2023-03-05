using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public List<Transform> spawnPoints = new List<Transform>();

    public List<GameObject> monsterPool = new List<GameObject>();
    public int maxMonsters = 10;

    public GameObject monsterPrefab;
    public float createTime = 3.0f;

    public TMP_Text scoreText;
    private int totalScore = 0;

    private bool isGameOver;
    public bool IsGameOver
    {
        get { return isGameOver; }
        set
        {
            isGameOver = value;
            if (isGameOver)
            {
                CancelInvoke(nameof(CreateMonster));
            }
        }
    }

    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if( instance != this)
        { 
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        CreateMonsterPool();
        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup")?.transform;

        foreach(Transform spawnPoint in spawnPointGroup)
        {
            spawnPoints.Add(spawnPoint);
        }

        InvokeRepeating(nameof(CreateMonster), 2.0f, createTime);

        totalScore = PlayerPrefs.GetInt("TOTAL_SCORE", 0);
        DisplayScore(0);
    }

    void CreateMonsterPool()
    {
        for(int i = 0; i < maxMonsters; ++i)
        {
            var _monster = Instantiate<GameObject>(monsterPrefab);

            _monster.name = $"Monster_{i:00}";

            _monster.SetActive(false);
            monsterPool.Add(_monster);
        }
    }

    private GameObject GetMonsterInPool()
    {
        foreach (GameObject monster in monsterPool)
        {
            if(monster.activeSelf == false)
            {
                return monster;
            }
        }
        return null;
    }

    void CreateMonster()
    {
        int idx = Random.Range(0, spawnPoints.Count);
        GameObject _monster = GetMonsterInPool();
        _monster?.transform.SetPositionAndRotation(spawnPoints[idx].position,
            spawnPoints[idx].rotation);
        _monster.SetActive(true);
    }

    public void DisplayScore(int score)
    {
        totalScore += score;
        scoreText.text = $"<color=#00ff00>SCORE :</color> <color=#ff0000>{totalScore:#,##0}</color>";

        PlayerPrefs.SetInt("TOTAL_SCORE", totalScore);
    }
}
