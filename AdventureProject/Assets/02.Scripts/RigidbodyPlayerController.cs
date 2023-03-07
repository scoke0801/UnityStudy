using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class RigidbodyPlayerController : MonoBehaviour
{
    #region 
    [Serializable]
    public class Components
    {
        public CapsuleCollider capsule;
        public Rigidbody rigidbody;
        public Animator animator;
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
        public KeyCode attack = KeyCode.Mouse0;
    }

    [Serializable]
    public class CheckOption
    {
        [Tooltip("�������� üũ�� ���̾� ����")]
        public LayerMask groundLayerMask = -1;

        [Range(0.01f, 0.5f), Tooltip("���� ���� �Ÿ�")]
        public float forwardCheckDistance = 0.1f;

        [Range(0.1f, 10.0f), Tooltip("���� ���� �Ÿ�")]
        public float groundCheckDistance = 2.0f;

        [Range(0.0f, 0.1f), Tooltip("���� �ν� ��� �Ÿ�")]
        public float groundCheckThreshold = 0.01f;
    }

    [Serializable]
    public class MovementOption
    {
        [Range(1f, 10f), Tooltip("�̵��ӵ�")]
        public float moveSpeed = 2f;

        [Range(1f, 10f), Tooltip("�޸��� �̵��ӵ� ���� ���")]
        public float runSpeed = 2f * 1.8f;

        [Range(1f, 10f), Tooltip("���� ����")]
        public float jumpForce = 4.2f;

        [Range(0.0f, 2.0f), Tooltip("���� ��Ÿ��")]
        public float jumpCooldown = 0.6f;

        [Range(0, 3), Tooltip("���� ��� Ƚ��")]
        public int maxJumpCount = 1;

        [Range(1f, 75f), Tooltip("��� ������ ��簢")]
        public float maxSlopeAngle = 50f;

        [Range(0f, 4f), Tooltip("���� �̵��ӵ� ��ȭ��(����/����)")]
        public float slopeAccel = 1f;

        [Range(-9.81f, 0f), Tooltip("�߷�")]
        public float gravity = -9.81f;
        
        [Range(1f, 10f), Tooltip("���� ����")]
        public float jumpHeight = 1.2f;

        [Range(0.0f, 0.3f), Tooltip("ȸ�� �������� �̵��� �ӵ�")]
        public float rotationSmoothTime = 0.12f;

        [Space(10)]
        [Tooltip("���� ��� �ð�")]
        public float jumpTimeout = 0.50f;

        [Tooltip("���� ���ð�")]
        public float fallTimeout = 0.15f;

        [Tooltip("���� �� ����")]
        public float speedChanageRate = 10.0f;

        [Tooltip("���ܼӵ�")]
        public float terminalVelocity = 53.0f;

        [Tooltip("������� ����Ȯ�ο� ������")]
        public float groundOffset = -0.05f;

        [Tooltip("������� ���� Ȯ�ο� ������, ĳ���� ��Ʈ�ѷ��� �������� ��ġ��ų ��")]
        public float groundRadius = 0.18f;

    }
    [Serializable]
    public class CurrentState
    {
        public bool isMoving;
        public bool isRunning;
        public bool isGrounded;
        public bool isJump;
        public bool isOnSteepSlope;   // ��� �Ұ����� ���ο� �ö�� ����
        public bool isJumpTriggered;
        public bool isJumping;
        public bool isForwardBlocked; // ���濡 ��ֹ� ����
        public bool isOutOfControl;   // ���� �Ұ� ����
    }

    [Serializable]
    public class CurrentValue
    {
        public Vector3 moveDir;
        public Vector3 groundNormal;
        public Vector3 groundCross;
        public Vector2 look;

        public float horizontalVelocity;
        public float rotationVelocity;
        public float verticalVelocity = 0.0f;

        [Space]
        public float jumpCooldown;
        public int jumpCount;
        public float outOfControllDuration;

        [Space]
        public float groundDistance;
        public float groundSlopeAngle;         // ���� �ٴ��� ��簢
        public float groundVerticalSlopeAngle; // �������� �������� ��簢
        public float forwardSlopeAngle; // ĳ���Ͱ� �ٶ󺸴� ������ ��簢
        public float slopeAccel;        // ���� ���� ����/���� ����

        [Space]
        public float targetRotationY = 0.0f;
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


    #region .

    [SerializeField] private Components _components = new Components();
    [SerializeField] private CheckOption _checkOptions = new CheckOption();
    [SerializeField] private MovementOption _moveOptions = new MovementOption();
    [SerializeField] private CurrentState _currentStates = new CurrentState();
    [SerializeField] private CurrentValue _currentValues = new CurrentValue();

    private Components Com => _components;
    private CheckOption COption => _checkOptions;
    private MovementOption MoveOption => _moveOptions;
    public CurrentState State => _currentStates;
    private CurrentValue Current => _currentValues;
    public KeyOptions Key => _keyOption;
    [SerializeField]
    private KeyOptions _keyOption = new KeyOptions();

    public CameraOption CamOption => _cameraOption;
    [SerializeField]
    private CameraOption _cameraOption = new CameraOption();


    private float _capsuleRadiusDiff;
    private float _fixedDeltaTime;

    private float _castRadius; // Sphere, Capsule ����ĳ��Ʈ ������
    private Vector3 CapsuleTopCenterPoint
        => new Vector3(transform.position.x, transform.position.y + Com.capsule.height - Com.capsule.radius, transform.position.z);
    private Vector3 CapsuleBottomCenterPoint
        => new Vector3(transform.position.x, transform.position.y + Com.capsule.radius, transform.position.z);

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // animation id
    private int _animHashSpeed;
    private int _animHashGrounded;
    private int _animHashJump;
    private int _animHashFall;
    private int _animHashMotionSpeed;
    private int _animHashAttack;

    private float _animationBlend;

    private const float _camRotationThresHold = 0.01f;

    private float _jumpTimeoutDelta;
    private float _fallTimeOutDelta;
    #endregion

    #region .
    private IEnumerator Start()
    {
        TryGetComponent(out Com.animator);

        Com.mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        _cinemachineTargetYaw = CamOption.cinemachineCameraTarget.transform.rotation.eulerAngles.y;

        InitRigidbody();
        InitCapsuleCollider();

        AssignAnimationHashs();

        _jumpTimeoutDelta = MoveOption.jumpTimeout;
        _fallTimeOutDelta = MoveOption.fallTimeout;

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
        _animHashAttack = Animator.StringToHash("Attack");
    }
    private void InitRigidbody()
    {
        TryGetComponent(out Com.rigidbody);

        // ȸ���� �ڽ� Ʈ�������� ���� ���� ������ ���̱� ������ ������ٵ� ȸ���� ����
        Com.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        Com.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        Com.rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Com.rigidbody.useGravity = false; // �߷� ���� ����
    }

    private void InitCapsuleCollider()
    {
        TryGetComponent(out Com.capsule);

        _castRadius = Com.capsule.radius * 0.9f;
        _capsuleRadiusDiff = Com.capsule.radius - _castRadius + 0.05f;
    }

    #endregion

    #region .
    private void Update()
    {
        HandleKeyInput();
    }

    private void FixedUpdate()
    {
        _fixedDeltaTime = Time.fixedDeltaTime;

        JumpAndGravity();
        CheckGround();
        CheckForward();

        UpdateValues();

        CalculateRotation();
        CalculateMovements();
        ApplyMovementsToRigidbody();
    }

    private void LateUpdate()
    {
        RotateCamera();
    }

    void RotateCamera()
    {
        if (Current.look.sqrMagnitude >= _camRotationThresHold && CamOption.isCameraLocked == false)
        {
            _cinemachineTargetYaw += Current.look.x;
            _cinemachineTargetPitch += Current.look.y;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, CamOption.bottomClamp, CamOption.topClamp);

        CamOption.cinemachineCameraTarget.transform.rotation = Quaternion.Euler(
            _cinemachineTargetPitch + CamOption.cameraAngleOverride,
            _cinemachineTargetYaw,
            0.0f);
    }

    private void HandleKeyInput()
    {
        float h = 0f, v = 0f;

        if (Input.GetKey(Key.moveForward)) { v += 1.0f; }
        if (Input.GetKey(Key.moveBackward)) { v -= 1.0f; }
        if (Input.GetKey(Key.moveLefet)) { h -= 1.0f; }
        if (Input.GetKey(Key.moveRight)) { h += 1.0f; }

        Current.moveDir = new Vector3(h, 0f, v).normalized;

        Current.look = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"));

        State.isMoving = h != 0 || v != 0;
        State.isRunning = Input.GetKey(Key.run);
        State.isJump = Input.GetKey(Key.jump);

        if (Input.GetKeyDown(Key.attack))
        {
            Com.animator.SetTrigger(_animHashAttack);
        }

        if( Input.GetKey(Key.jump))
        {
            SetJump();
        }
    }

    #endregion
    /***********************************************************************
    *                               Public Methods
    ***********************************************************************/
    #region .

    bool SetJump()
    {
        // ù ������ ���� �������� ����
        if (!State.isGrounded && Current.jumpCount == 0) return false;

        // ���� ��Ÿ��, Ƚ�� Ȯ��
        if (Current.jumpCooldown > 0f) return false;
        if (Current.jumpCount >= MoveOption.maxJumpCount) return false;

        // ���� �Ұ��� ���ο��� ���� �Ұ���
        if (State.isOnSteepSlope) return false;

        State.isJumpTriggered = true;
        return true;
    }


    void KnockBack(in Vector3 force, float time)
    {
        SetOutOfControl(time);
        Com.rigidbody.AddForce(force, ForceMode.Impulse);
    }

    public void SetOutOfControl(float time)
    {
        Current.outOfControllDuration = time;
        ResetJump();
    }

    #endregion
    /***********************************************************************
    *                               Private Methods
    ***********************************************************************/
    #region .

    private void ResetJump()
    {
        Current.jumpCooldown = 0f;
        Current.jumpCount = 0;
        State.isJumping = false;
        State.isJumpTriggered = false;
    }

    void JumpAndGravity()
    {
        if (State.isGrounded)
        {
            _fallTimeOutDelta = MoveOption.fallTimeout;

            Com.animator.SetBool(_animHashJump, false);
            Com.animator.SetBool(_animHashFall, false);

            if (Current.verticalVelocity < 0.0f)
            {
                Current.verticalVelocity = -2f;
            }

            // Jump here
            if (State.isJump && _jumpTimeoutDelta <= 0.0f)
            {
                Debug.Log("Jump!");
                // sqaure of H * -2 * G, �󸶳� ���� �� ������.
                Current.verticalVelocity = Mathf.Sqrt(MoveOption.jumpHeight * -2f * MoveOption.gravity);

                Com.animator.SetBool(_animHashJump, true);
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = MoveOption.jumpTimeout;

            if (_fallTimeOutDelta >= 0.0f)
            {
                _fallTimeOutDelta -= Time.deltaTime;
            }
            else
            {
                Com.animator.SetBool(_animHashFall, true);
            }

            State.isJump = false;
        }

        if (Current.verticalVelocity < MoveOption.terminalVelocity)
        {
            Current.verticalVelocity += MoveOption.gravity * Time.deltaTime;
        }
    }

    private void CheckGround()
    {
        Current.groundDistance = float.MaxValue;
        Current.groundNormal = Vector3.up;
        Current.groundSlopeAngle = 0f;
        Current.forwardSlopeAngle = 0f;
        
        bool cast =
            Physics.SphereCast(CapsuleBottomCenterPoint, _castRadius, Vector3.down, out var hit, COption.groundCheckDistance, COption.groundLayerMask, QueryTriggerInteraction.Ignore);
             State.isGrounded = false;

        if (cast)
        {
            // ���� ��ֺ��� �ʱ�ȭ
            Current.groundNormal = hit.normal;

            // ���� ��ġ�� ������ ��簢 ���ϱ�(ĳ���� �̵����� ���)
            Current.groundSlopeAngle = Vector3.Angle(Current.groundNormal, Vector3.up);

            Current.forwardSlopeAngle = Vector3.Angle(Current.groundNormal, transform.forward) - 90f;

            State.isOnSteepSlope = Current.groundSlopeAngle >= MoveOption.maxSlopeAngle;
                
            Current.groundDistance = Mathf.Max(hit.distance - _capsuleRadiusDiff - COption.groundCheckThreshold, 0f);
        }
        
        State.isGrounded = (Current.groundDistance <= 0.0001f) && !State.isOnSteepSlope;

        // ���� �̵����� ȸ����
        Current.groundCross = Vector3.Cross(Current.groundNormal, Vector3.up);
        
        Com.animator.SetBool(_animHashGrounded, State.isGrounded);
    }

    private void CheckForward()
    {
        bool cast =
            Physics.CapsuleCast(CapsuleBottomCenterPoint, CapsuleTopCenterPoint, _castRadius, transform.forward + Vector3.down * 0.1f,
                out var hit, COption.forwardCheckDistance, -1, QueryTriggerInteraction.Ignore);

        State.isForwardBlocked = false;
        if (cast)
        {
            float forwardObstacleAngle = Vector3.Angle(hit.normal, Vector3.up);
            State.isForwardBlocked = forwardObstacleAngle >= MoveOption.maxSlopeAngle;

            Debug.Log("Forward Block~!");
        }
    }

    private void UpdateValues()
    {
        // Calculate Jump Cooldown
        if (Current.jumpCooldown > 0f)
        {
            Current.jumpCooldown -= _fixedDeltaTime;
        }

        // Out Of Control
        State.isOutOfControl = Current.outOfControllDuration > 0f;

        if (State.isOutOfControl)
        {
            Current.outOfControllDuration -= _fixedDeltaTime;
            Current.moveDir = Vector3.zero;
        }
    }

    private void CalculateRotation()
    {
        if (State.isMoving == false) { return; }

        Current.targetRotationY = Mathf.Atan2(Current.moveDir.x, Current.moveDir.z ) * Mathf.Rad2Deg +
                  Com.mainCamera.transform.eulerAngles.y;

        float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y, Current.targetRotationY,
            ref Current.rotationVelocity, MoveOption.rotationSmoothTime);

        transform.rotation = Quaternion.Euler(0.0f, rotationY, 0.0f);
    }

    private void CalculateMovements()
    {
        if (State.isOutOfControl)
        {
            Current.horizontalVelocity = 0.0f;
            return;
        }

        // 1. ����
        if (State.isJumpTriggered && Current.jumpCooldown <= 0f)
        {
            // ���� ��Ÿ��, Ʈ���� �ʱ�ȭ
            Current.jumpCooldown = MoveOption.jumpCooldown;
            State.isJumpTriggered = false;
            State.isJumping = true;

            Current.jumpCount++;

            Com.animator.SetBool(_animHashJump, true);
        }

        // 2. XZ �̵��ӵ� ���
        // ���߿��� ������ ���� ��� ���� (���󿡼��� ���� �پ �̵��� �� �ֵ��� ���)
        if (State.isForwardBlocked && !State.isGrounded || State.isJumping && State.isGrounded)
        {
            Current.horizontalVelocity = 0.0f;
        }
        else // �̵� ������ ��� : ���� or ������ ������ ����
        {
            float speed = !State.isMoving ? 0f :
                          !State.isRunning ? MoveOption.moveSpeed :
                                             MoveOption.runSpeed;

            float currentHorizontalSpeed = new Vector3(Com.rigidbody.velocity.x, 0.0f, Com.rigidbody.velocity.z).magnitude;
            float speedOffset = 0.1f;
            if (currentHorizontalSpeed < speed - speedOffset ||
                currentHorizontalSpeed > speed + speedOffset)
            {
                // ���� �������� �ӵ� ��ȭ�� �����ϴ� ���� ����� �ƴ� � ����� �����մϴ�.
                // ���� Lerp�� T�� �����Ǿ� �����Ƿ� �ӵ��� ������ �ʿ䰡 �����ϴ�.
                Current.horizontalVelocity = Mathf.Lerp(currentHorizontalSpeed, speed, Time.deltaTime * MoveOption.speedChanageRate);

                Current.horizontalVelocity = Mathf.Round(Current.horizontalVelocity * 1000f) / 1000f;
            }
            else
            {
                Current.horizontalVelocity = speed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, speed, Time.deltaTime * 10f);
            if (_animationBlend < 0.01f) { _animationBlend = 0.0f; }
            
            Com.animator.SetFloat(_animHashSpeed, _animationBlend);
            Com.animator.SetFloat(_animHashMotionSpeed, 1.0f);
        }

        // 3. XZ ���� ȸ��
        // �����̰ų� ���鿡 ����� ����
        if (State.isGrounded || Current.groundDistance < COption.groundCheckDistance && !State.isJumping)
        {
            if (State.isMoving && !State.isForwardBlocked)
            {
                // ���� ���� ����/����
                if (MoveOption.slopeAccel > 0f)
                {
                    bool isPlus = Current.forwardSlopeAngle >= 0f;
                    float absFsAngle = isPlus ? Current.forwardSlopeAngle : -Current.forwardSlopeAngle;
                    float accel = MoveOption.slopeAccel * absFsAngle * 0.01111f + 1f;
                    Current.slopeAccel = !isPlus ? accel : 1.0f / accel;

                    Current.horizontalVelocity *= Current.slopeAccel;
                }

                // Fix Here
                // ���� ȸ�� (����)
                //Current.horizontalVelocity =
                //    Quaternion.AngleAxis(-Current.groundSlopeAngle, Current.groundCross) * Current.horizontalVelocity;
            }
        }
    }

    private void ApplyMovementsToRigidbody()
    {
        if (State.isOutOfControl)
        {
            Com.rigidbody.velocity = new Vector3(Com.rigidbody.velocity.x, Current.verticalVelocity, Com.rigidbody.velocity.z);
            return;
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, Current.targetRotationY, 0.0f) * Vector3.forward;

        Com.rigidbody.velocity = targetDirection.normalized * (Current.horizontalVelocity * _fixedDeltaTime) +
            new Vector3(0.0f, Current.verticalVelocity, 0.0f) * _fixedDeltaTime;
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
    #endregion
}

