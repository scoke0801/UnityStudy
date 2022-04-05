using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public ItemBox[] itemBoxes;

    public GameObject winUI;

    public bool isGameOver;
    // Start is called before the first frame update
    void Start()
    { 
        isGameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Main");
        }

        if ( isGameOver )
        {
            return;
        }

        isGameOver = true;
        for (int i = 0; i < itemBoxes.Length; ++i)
        {
            if ( !itemBoxes[i].isOverlapped )
            {
                isGameOver = false;
            }
        }
        if ( isGameOver )
        {
            winUI.SetActive(true);
            Debug.Log("°ÔÀÓ ½Â¸®!");
        }
    }
}
