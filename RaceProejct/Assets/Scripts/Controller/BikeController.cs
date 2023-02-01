using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BikeController : ObserverSubject
{
    // for event Test 
    private string _status;

    public float maxSpeed = 2.0f;
    public float turnDistance = 2.0f;

    public float CurrentSpeed { get; set; }
    public Direction CurrentTurnDirection { get; private set; }

    private IBikeState _startState, _stopState, _turnState;
    private BikeStateContext _bikeStateContext;

    public bool IsTurboOn { get; private set; }

    public float CurrentHealth {  get { return _health; } }

    [SerializeField]
    private float _health;

    private bool _isEngineOn;
    private HUDController _hudController;
    private CameraController _cameraController;
    
    private float _distance = 1.0f;

    private void OnEnable()
    {
        RaceEventBus.Subscribe(RaceEventType.START, StartBike);
        RaceEventBus.Subscribe(RaceEventType.STOP, StopBike);

        if (_hudController)
        {
            Attach(_hudController);
        }

        if (_cameraController)
        {
            Attach(_cameraController);
        }
    }
    private void OnDisable()
    {
        RaceEventBus.Unsubscribe(RaceEventType.START, StartBike);
        RaceEventBus.Unsubscribe(RaceEventType.STOP, StopBike);

        if (_hudController)
        {
            Detach(_hudController);
        }

        if (_cameraController)
        {
            Detach(_cameraController);
        }
    }

    private void Awake()
    {
        _hudController = gameObject.AddComponent<HUDController>();
        _cameraController = (CameraController)FindObjectOfType(typeof(CameraController));
    }
    private void Start()
    {
        _bikeStateContext = new BikeStateContext(this);

        _startState = gameObject.AddComponent<BikeStartState>();
        _stopState = gameObject.AddComponent<BikeStopState>();
        _turnState = gameObject.AddComponent<BikeTurnState>();

        _bikeStateContext.Transition(_stopState);

        StartEngine();
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
        if (_isEngineOn)
        {
            IsTurboOn = !IsTurboOn;
            DebugWrapper.Log("Turbo Active: " + IsTurboOn.ToString());
        }

        NotifyObservers();
    }
    
    //private void OnGUI()
    //{
    //    GUI.color = Color.green;
    //    GUI.Label(new Rect(10, 60, 200, 20), "BIKE STATUS:" + _status);
    //}

    void StartEngine()
    {
        _isEngineOn = true;
        NotifyObservers();
    }

    public void TakeDamage(float amount)
    {
        _health -= amount;
        IsTurboOn = false;

        NotifyObservers();

        if(_health < 0)
        {
            Destroy(gameObject);
        }
    }
}

