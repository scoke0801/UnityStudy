using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : Observer
{
    private bool _isDisplayOn;

    private bool _isTurboOn;
    private float _currentHealth;
    private BikeController _bikeController;


    void OnEnable()
    {
        RaceEventBus.Subscribe(RaceEventType.START, DisplayHUD);
    }
    private void OnDisable()
    {
        RaceEventBus.Unsubscribe(RaceEventType.START, DisplayHUD);
    }

    private void DisplayHUD()
    {
        _isDisplayOn = true;
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(50, 50, 100, 200));

        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Health: " + _currentHealth);
        GUILayout.EndHorizontal();

        if (_isTurboOn)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Turbo Activated!");
            GUILayout.EndHorizontal();
        }

        if (_currentHealth <= 50.0f)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("WARNING: Low Health");
            GUILayout.EndHorizontal();
        }

        GUILayout.EndArea();

        if (_isDisplayOn)
        {
            if(GUILayout.Button("Stop Race"))
            {
                _isDisplayOn = false;
                RaceEventBus.Publish(RaceEventType.STOP);
            }
        }
    }

    public override void Notify(ObserverSubject subject)
    {
        if (!_bikeController)
        {
            _bikeController = subject.GetComponent<BikeController>();
        }

        if (_bikeController)
        {
            _isTurboOn = _bikeController.IsTurboOn;
            _currentHealth = _bikeController.CurrentHealth;
        }
    }
}
