using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BikeController : MonoBehaviour
{
    // for event Test 
    private string _status;

    public float maxSpeed = 2.0f;
    public float turnDistance = 2.0f;

    public float CurrentSpeed { get; set; }
    public Direction CurrentTurnDirection { get; private set; }

    private IBikeState _startState, _stopState, _turnState;
    private BikeStateContext _bikeStateContext;

    private bool _isTurboOn;
    private float _distance = 1.0f;

    private void OnEnable()
    {
        RaceEventBus.Subscribe(RaceEventType.START, StartBike);
        RaceEventBus.Subscribe(RaceEventType.STOP, StopBike);
    }
    private void OnDisable()
    {
        RaceEventBus.Unsubscribe(RaceEventType.START, StartBike);
        RaceEventBus.Unsubscribe(RaceEventType.STOP, StopBike);
    }

    private void Start()
    {
        _bikeStateContext = new BikeStateContext(this);

        _startState = gameObject.AddComponent<BikeStartState>();
        _stopState = gameObject.AddComponent<BikeStopState>();
        _turnState = gameObject.AddComponent<BikeTurnState>();

        _bikeStateContext.Transition(_stopState);
    }

    public void StartBike()
    {
        _bikeStateContext.Transition(_startState);
        _status = "Started";
    }

    public void StopBike()
    {
        _bikeStateContext.Transition(_stopState);
        _status = "Stopped";
    }

    public void Turn(Direction direction)
    {
        //if(direction == Direction.Left)
        //{
        //    transform.Translate(Vector3.left * _distance);
        //}
        //if(direction == Direction.Right)
        //{
        //    transform.Translate(Vector3.right * _distance);
        //}
       
        CurrentTurnDirection = direction;
        _bikeStateContext.Transition(_turnState);
    }

    public void ResetPosition()
    {
        transform.position = Vector3.zero;
    }
    public void ToggleTurbo()
    {
        _isTurboOn = !_isTurboOn;
        DebugWrapper.Log("Turbo Active: " + _isTurboOn.ToString());
    }
    
    //private void OnGUI()
    //{
    //    GUI.color = Color.green;
    //    GUI.Label(new Rect(10, 60, 200, 20), "BIKE STATUS:" + _status);
    //}
}

