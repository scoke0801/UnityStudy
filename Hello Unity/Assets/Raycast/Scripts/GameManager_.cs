using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager_ : MonoBehaviour
{
    public Transform randPos;
    public GameObject fightingCar;
    int numOfCar = 15;

    GameObject timeTextObject;
    int startTime = 0;
    int elapsedTime = 0;
    Text elapsedTimeText;

    // Start is called before the first frame update
    void Start()
    {
        for(int i =0; i < numOfCar; ++i)
        {
            randPos.position = new Vector3(Random.Range(-7f, 7f), 0, Random.Range(28f, 32f));
            Instantiate(fightingCar, randPos.position, randPos.rotation);
        }
        timeTextObject = GameObject.Find("Timer");
        elapsedTimeText = timeTextObject.GetComponent<Text>();

        elapsedTimeText.text = startTime.ToString();
        startTime = (int)Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = (int)Time.time - startTime;
        elapsedTimeText.text = "지나간 시간: " + elapsedTime.ToString();
        
    }
}
