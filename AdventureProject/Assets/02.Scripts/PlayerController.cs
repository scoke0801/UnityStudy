using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Class

    [Serializable]
    public class Components
    {
        public Animator anim;
        public CharacterController controller;
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
        [Range(1f, 10f), Tooltip("이동속도")]
        public float speed = 3f;

        [Range(1f, 3f), Tooltip("달리기 이동속도 증가 계수")]
        public float accelration = 1.5f;

        [Range(1f, 10f), Tooltip("점프 강도")]
        public float jumpForce = 5.5f;
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
        public bool isGrounded;
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

    private Vector3 _moveDir;
    private Vector3 _worldMove;
    private Vector2 _rotation;
    #endregion

    private void Awake()
    {
        Com.anim = GetComponent<Animator>();
        Com.controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleKeyInput();
        Rotate();
        Move();
    }

    private void HandleKeyInput()
    {
        float h = 0f, v = 0f;

        if (Input.GetKey(Key.moveForward)) { v += 1.0f; }
        if( Input.GetKey(Key.moveBackward)) { v -= 1.0f; }
        if( Input.GetKey(Key.moveLefet)) { h -= 1.0f; }
        if (Input.GetKey(Key.moveRight)) { h += 1.0f; }

        Vector3 moveInput = new Vector3(h, 0f, v).normalized;
        _moveDir = Vector3.Lerp(_moveDir, moveInput, MoveOption.accelration);
        _rotation = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));

        State.isMoving = _moveDir.magnitude > 0.01f;
        State.isRunning = Input.GetKey(Key.run);
    }

    private void Rotate()
    {

    }

    private void Move()
    {
        if (State.isMoving == false )
        {
            return;
        }

        _moveDir *= MoveOption.speed * (State.isRunning ? MoveOption.accelration : 1.0f);

        Com.controller.Move(_moveDir * Time.deltaTime);
    }
}
