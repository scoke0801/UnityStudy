using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Class

    [Serializable]
    public class Components
    {
        public Animator animator;
        public CharacterController controller;

        public GameObject mainCamera;
    }

    [Serializable]
    public class KeyOptions
    {
        public KeyCode moveForward = KeyCode.W;
        public KeyCode moveBackward = KeyCode.S;
        public KeyCode moveLefet = KeyCode.A;
        public KeyCode moveRight = KeyCode.D;
        public KeyCode run = KeyCode.LeftShift;
        public KeyCode jump = KeyCode.Space;
        public KeyCode showCursor = KeyCode.LeftAlt;
    }

    [Serializable]
    public class MovementOption
    {
        [Range(1f, 5f), Tooltip("�̵��ӵ�")]
        public float moveSpeed = 2f;

        [Range(1f, 10f), Tooltip("�޸��� �̵��ӵ� ���� ���")]
        public float runSpeed = 2f * 1.8f;

        [Range(0.0f, 0.3f), Tooltip("ȸ�� �������� �̵��� �ӵ�")]
        public float rotationSmoothTime = 0.12f;

        [Tooltip("������� ����Ȯ�ο� ������")]
        public float groundOffset = -0.05f;

        [Tooltip("������� ���� Ȯ�ο� ������, ĳ���� ��Ʈ�ѷ��� �������� ��ġ��ų ��")]
        public float groundRadius = 0.5f;

        [Tooltip("�������� �Ǵ��� ���̾�")]
        public LayerMask groundLayers;

        [Range(1f, 10f), Tooltip("���� ����")]
        public float jumpHeight = 1.2f;

        [Tooltip("�߷�")]
        public float gravity = -9.8f;

        [Space(10)]
        [Tooltip("���� ��� �ð�")]
        public float jumpTimeout = 0.50f;

        [Tooltip("���� ���ð�")]
        public float fallTimeout = 0.15f;

        [Tooltip("���� �� ����")]
        public float speedChanageRate = 10.0f;

        [Tooltip("���ܼӵ�")]
        public float terminalVelocity = 53.0f;
    }

    [Serializable]
    public class AnimationOption
    {
        public string paramMove = "Move";
    }

    [Serializable]
    public class PlayerState
    {
        public bool isMoving;
        public bool isRunning;
        public bool isGrounded = true;
        public bool isJump = false;
    }

    [Serializable]
    public class CameraOption
    {
        public GameObject cinemachineCameraTarget;

        [Tooltip("ī�޶� �����ٺ� �� �ִ� �ִ� ����")]
        public float bottomClamp = -30.0f;

        [Tooltip("ī�޶� �÷��ٺ� �� �ִ� �ִ� ����")]
        public float topClamp = 70.0f;

        [Tooltip("ī�޶� �������ϴ� �߰� ����")]
        public float cameraAngleOverride = 0.0f;

        [Tooltip("ī�޶� ��� ����")]
        public bool isCameraLocked = false;
    }
    #endregion

    #region Variables
    public Components Com => _components;
    [SerializeField]
    private Components _components = new Components();

    public KeyOptions Key => _keyOption;
    [SerializeField]
    private KeyOptions _keyOption = new KeyOptions();

    public MovementOption MoveOption => _moveOption;
    [SerializeField]
    private MovementOption _moveOption = new MovementOption();

    public AnimationOption AnimOption => _animationOption;
    [SerializeField]
    private AnimationOption _animationOption;

    public PlayerState State => _state;
    [SerializeField]
    private PlayerState _state = new PlayerState();

    public CameraOption CamOption => _cameraOption;
    [SerializeField]
    private CameraOption _cameraOption = new CameraOption();

    private float _speed;
    private float _animationBlend;
    private Vector3 _moveDir;
    private Vector3 _worldMove;
    private Vector2 _look;
    private float _targetRotationY = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;

    private float _jumpTimeoutDelta;
    private float _fallTimeOutDelta;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // animation id
    private int _animHashSpeed;
    private int _animHashGrounded;
    private int _animHashJump;
    private int _animHashFall;
    private int _animHashMotionSpeed;

    private const float _camRotationThresHold = 0.01f;

    #endregion

    private void Awake()
    {
        Com.animator = GetComponentInChildren<Animator>();
        Com.controller = GetComponent<CharacterController>();
        Com.mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private IEnumerator Start()
    {
        _jumpTimeoutDelta = MoveOption.jumpTimeout;
        _fallTimeOutDelta = MoveOption.fallTimeout;

        _cinemachineTargetYaw = CamOption.cinemachineCameraTarget.transform.rotation.eulerAngles.y;

        AssignAnimationHashs();

        CamOption.isCameraLocked = true;
        yield return new WaitForSeconds(0);
        CamOption.isCameraLocked = false;
    }

    void AssignAnimationHashs()
    {
        _animHashSpeed = Animator.StringToHash("Speed");
        _animHashGrounded = Animator.StringToHash("Grounded");
        _animHashJump = Animator.StringToHash("Jump");
        _animHashFall = Animator.StringToHash("Falling");
        _animHashMotionSpeed = Animator.StringToHash("MotionSpeed");

    }

    void Update()
    {
        HandleKeyInput();

        JumpAndGravity();
        GroundedCheck();
        Rotate();
        Move();
    }

    private void LateUpdate()
    {
        RotateCamera();
    }

    private void HandleKeyInput()
    {
        float h = 0f, v = 0f;

        if (Input.GetKey(Key.moveForward)) { v += 1.0f; }
        if( Input.GetKey(Key.moveBackward)) { v -= 1.0f; }
        if( Input.GetKey(Key.moveLefet)) { h -= 1.0f; }
        if (Input.GetKey(Key.moveRight)) { h += 1.0f; }

        _moveDir = new Vector3(h, 0f, v).normalized;

        _look = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));

        State.isMoving = h != 0 || v != 0;
        State.isRunning = Input.GetKey(Key.run);
        State.isJump = Input.GetKey(Key.jump);
    }

    private void Rotate()
    {
        if(State.isMoving == false) { return; }

        _targetRotationY = Mathf.Atan2(_moveDir.x, _moveDir.z) * Mathf.Rad2Deg +
                   Com.mainCamera.transform.eulerAngles.y;

        float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotationY,
            ref _rotationVelocity, MoveOption.rotationSmoothTime);

        transform.rotation = Quaternion.Euler(0.0f, rotationY, 0.0f);
    }

    private void Move()
    {
        float targetSpeed = State.isRunning ? MoveOption.runSpeed : MoveOption.moveSpeed;
        
        if( State.isMoving == false) { targetSpeed = 0.0f; }

        float currentHorizontalSpeed = new Vector3(Com.controller.velocity.x, 0.0f, Com.controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // ���� �������� �ӵ� ��ȭ�� �����ϴ� ���� ����� �ƴ� � ����� �����մϴ�.
            // ���� Lerp�� T�� �����Ǿ� �����Ƿ� �ӵ��� ������ �ʿ䰡 �����ϴ�.
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * MoveOption.speedChanageRate);

            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * MoveOption.speedChanageRate);
        if(_animationBlend < 0.01f) { _animationBlend = 0.0f; }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotationY, 0.0f) * Vector3.forward;

        Com.controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
            new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        Com.animator.SetFloat(_animHashSpeed, _animationBlend);
        Com.animator.SetFloat(_animHashMotionSpeed, 1.0f);
    }

    void JumpAndGravity()
    {
        if(State.isGrounded)
        {
            _fallTimeOutDelta = MoveOption.fallTimeout;

            Com.animator.SetBool(_animHashJump, false);
            Com.animator.SetBool(_animHashFall, false);

            if( _verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump here
            if( State.isJump && _jumpTimeoutDelta <= 0.0f)
            {
                // sqaure of H * -2 * G, �󸶳� ���� �� ������.
                _verticalVelocity = Mathf.Sqrt(MoveOption.jumpHeight * -2f * MoveOption.gravity);

                Com.animator.SetBool(_animHashJump, true);
            }

            if(_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = MoveOption.jumpTimeout;

            if(_fallTimeOutDelta >= 0.0f)
            {
                _fallTimeOutDelta -= Time.deltaTime;
            }
            else
            {
                Com.animator.SetBool(_animHashFall, true);
            }

            _state.isJump = false;
        }

        if(_verticalVelocity < MoveOption.terminalVelocity)
        {
            _verticalVelocity += MoveOption.gravity * Time.deltaTime;
        }
    }

    void GroundedCheck()
    {
        if(_look.sqrMagnitude >= _camRotationThresHold && CamOption.isCameraLocked == false )
        {
            _cinemachineTargetYaw += _look.x;
            _cinemachineTargetPitch += _look.y;
        }

        Vector3 spherePosition = new Vector3(transform.position.x,
            transform.position.y - MoveOption.groundOffset, transform.position.z);
        State.isGrounded = Physics.CheckSphere(spherePosition, MoveOption.groundRadius, MoveOption.groundLayers,
            QueryTriggerInteraction.Ignore);

        Com.animator.SetBool(_animHashGrounded, State.isGrounded);
    }

    void RotateCamera()
    {
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, CamOption.bottomClamp, CamOption.topClamp);

        CamOption.cinemachineCameraTarget.transform.rotation = Quaternion.Euler(
            _cinemachineTargetPitch + CamOption.cameraAngleOverride,
            _cinemachineTargetYaw, 
            0.0f);
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360.0f) { angle += 360.0f; }
        if (angle > 360.0f) { angle -= 360.0f; }
        return Mathf.Clamp(angle, min, max);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (State.isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - MoveOption.groundOffset, transform.position.z),
            MoveOption.groundRadius);
    }
    private void OnLand(AnimationEvent animationEvent)
    {
       
    }

}
