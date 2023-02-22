using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button startButton;
    public Button optionButton;
    public Button shopButton;

    private UnityAction action;

    private void Start()
    {
        // Unity Action.
        action = ()=> OnButtonClick(startButton.name);
        startButton.onClick.AddListener(action);

        // 무명 메서드.
        optionButton.onClick.AddListener(delegate { OnButtonClick(optionButton.name); });

        // 람다식.
        shopButton.onClick.AddListener(() => OnButtonClick(shopButton.name));
    }

    public void OnButtonClick(string msg)
    {
        Debug.Log($"ClickButton: {msg}");
    }
}
