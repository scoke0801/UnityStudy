using System.Collections;
using UnityEngine;

public class ClientEventBus : MonoBehaviour
{
    private bool _isButtonEnabled;

    // Use this for initialization
    void Start()
    {
        gameObject.AddComponent<HUDController>();
        gameObject.AddComponent<CountdonwTimer>();
        gameObject.AddComponent<BikeController>();

        _isButtonEnabled = true;
    }

    private void OnEnable()
    {
        RaceEventBus.Subscribe(RaceEventType.STOP, Restart);
    }

    private void OnDisable()
    {
        RaceEventBus.Unsubscribe(RaceEventType.STOP, Restart);
    }

    private void Restart()
    {
        _isButtonEnabled = true;
    }

    private void OnGUI()
    {
        if (_isButtonEnabled)
        {
            if(GUILayout.Button("Start Countdown"))
            {
                _isButtonEnabled = false;
                RaceEventBus.Publish(RaceEventType.COUNTDOWN);
            }
        }
    }
}   