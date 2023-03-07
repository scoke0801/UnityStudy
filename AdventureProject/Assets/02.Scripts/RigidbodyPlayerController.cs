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
        [Tooltip("지면으로 체크할 레이어 설정")]
        public LayerMask groundLayerMask = -1;

        [Range(0.01f, 0.5f), Tooltip("전방 감지 거리")]
        public float forwardCheckDistance = 0.1f;

        [Range(0.1f, 10.0f), Tooltip("지면 감지 거리")]
        public float groundCheckDistance = 2.0f;

        [Range(0.0f, 0.1f), Tooltip("지면 인식 허용 거리")]
        public float groundCheckThreshold = 0.01f;
    }

    [Serializable]
    public class MovementOption
    {
        [Range(1f, 10f), Tooltip("이동속도")]
        public float moveSpeed = 2f;

        [Range(1f, 10f), Tooltip("달리기 이동속도 증가 계수")]
        public float runSpeed = 2f * 1.8f;

        [Range(1f, 10f), Tooltip("점프 강도")]
        public float jumpForce = 4.2f;

        [Range(0.0f, 2.0f), Tooltip("점프 쿨타임")]
        public float jumpCooldown = 0.6f;

        [Range(0, 3), Tooltip("점프 허용 횟수")]
        public int maxJumpCount = 1;

        [Range(1f, 75f), Tooltip("등반 가능한 경사각")]
        public float maxSlopeAngle = 50f;

        [Range(0f, 4f), Tooltip("경사로 이동속도 변화율(가속/감속)")]
        public float slopeAccel = 1f;

        [Range(-9.81f, 0f), Tooltip("중력")]
        public float gravity = -9.81f;
        
        [Range(1f, 10f), Tooltip("점프 높이")]
        public float jumpHeight = 1.2f;

        [Range(0.0f, 0.3f), Tooltip("회전 방향으로 이동할 속도")]
        public float rotationSmoothTime = 0.12f;

        [Space(10)]
        [Tooltip("점프 대기 시간")]
        public float jumpTimeout = 0.50f;

        [Tooltip("낙하 대기시간")]
        public float fallTimeout = 0.15f;

        [Tooltip("가속 및 감소")]
        public float speedChanageRate = 10.0f;

        [Tooltip("종단속도")]
        public float terminalVelocity = 53.0f;

        [Tooltip("지면과의 높이확인용 오프셋")]
        public float groundOffset = -0.05f;

        [Tooltip("지면과의 높이 확인용 반지름, 캐릭터 컨트롤러의 반지름과 일치시킬 것")]
        public float groundRadius = 0.18f;

    }
    [Serializable]
    public class CurrentState
    {
        public bool isMoving;
        public bool isRunning;
        public bool isGrounded;
        public bool isJump;
        public bool isOnSteepSlope;   // 등반 불가능한 경사로에 올라와 있음
        public bool isJumpTriggered;
        public bool isJumping;
        public bool isForwardBlocked; // 전방에 장애물 존재
        public bool isOutOfControl;   // 제어 불가 상태
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
        public float groundSlopeAngle;         // 현재 바닥의 경사각
        public float groundVerticalSlopeAngle; // 수직으로 재측정한 경사각
        public float forwardSlopeAngle; // 캐릭터가 바라보는 방향의 경사각
        public float slopeAccel;        // 경사로 인한 가속/감속 비율

        [Space]
        public float targetRotationY = 0.0f;
    }

    [Serializable]
    public class CameraOption
    {
        public GameObject cinemachineCameraTarget;

        [Tooltip("카메라를 내려다볼 수 있는 최대 각도")]
        public float bottomClamp = -30.0f;

        [Tooltip("카메라를 올려다볼 수 있는 최대 각도")]
        public float topClamp = 70.0f;

        [Tooltip("카메라 재정의하는 추가 각도")]
        public float cameraAngleOverride = 0.0f;

        [Tooltip("카메라 잠금 여부")]
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

    private float _castRadius; // Sphere, Capsule 레이캐스트 반지름
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

        // 회전은 자식 트랜스폼을 통해 직접 제어할 것이기 때문에 리지드바디 회전은 제한
        Com.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        Com.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        Com.rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Com.rigidbody.useGravity = false; // 중력 직접 제어
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
        // 첫 점프는 지면 위에서만 가능
        if (!State.isGrounded && Current.jumpCount == 0) return false;

        // 점프 쿨타임, 횟수 확인
        if (Current.jumpCooldown > 0f) return false;
        if (Current.jumpCount >= MoveOption.maxJumpCount) return false;

        // 접근 불가능 경사로에서 점프 불가능
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
                // sqaure of H * -2 * G, 얼마나 높이 뛸 것인지.
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
            // 지면 노멀벡터 초기화
            Current.groundNormal = hit.normal;

            // 현재 위치한 지면의 경사각 구하기(캐릭터 이동방향 고려)
            Current.groundSlopeAngle = Vector3.Angle(Current.groundNormal, Vector3.up);

            Current.forwardSlopeAngle = Vector3.Angle(Current.groundNormal, transform.forward) - 90f;

            State.isOnSteepSlope = Current.groundSlopeAngle >= MoveOption.maxSlopeAngle;
                
            Current.groundDistance = Mathf.Max(hit.distance - _capsuleRadiusDiff - COption.groundCheckThreshold, 0f);
        }
        
        State.isGrounded = (Current.groundDistance <= 0.0001f) && !State.isOnSteepSlope;

        // 월드 이동벡터 회전축
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

        // 1. 점프
        if (State.isJumpTriggered && Current.jumpCooldown <= 0f)
        {
            // 점프 쿨타임, 트리거 초기화
            Current.jumpCooldown = MoveOption.jumpCooldown;
            State.isJumpTriggered = false;
            State.isJumping = true;

            Current.jumpCount++;

            Com.animator.SetBool(_animHashJump, true);
        }

        // 2. XZ 이동속도 계산
        // 공중에서 전방이 막힌 경우 제한 (지상에서는 벽에 붙어서 이동할 수 있도록 허용)
        if (State.isForwardBlocked && !State.isGrounded || State.isJumping && State.isGrounded)
        {
            Current.horizontalVelocity = 0.0f;
        }
        else // 이동 가능한 경우 : 지상 or 전방이 막히지 않음
        {
            float speed = !State.isMoving ? 0f :
                          !State.isRunning ? MoveOption.moveSpeed :
                                             MoveOption.runSpeed;

            float currentHorizontalSpeed = new Vector3(Com.rigidbody.velocity.x, 0.0f, Com.rigidbody.velocity.z).magnitude;
            float speedOffset = 0.1f;
            if (currentHorizontalSpeed < speed - speedOffset ||
                currentHorizontalSpeed > speed + speedOffset)
            {
                // 보다 유기적인 속도 변화를 제공하는 선형 결과가 아닌 곡선 결과를 생성합니다.
                // 참고 Lerp의 T는 고정되어 있으므로 속도를 고정할 필요가 없습니다.
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

        // 3. XZ 벡터 회전
        // 지상이거나 지면에 가까운 높이
        if (State.isGrounded || Current.groundDistance < COption.groundCheckDistance && !State.isJumping)
        {
            if (State.isMoving && !State.isForwardBlocked)
            {
                // 경사로 인한 가속/감속
                if (MoveOption.slopeAccel > 0f)
                {
                    bool isPlus = Current.forwardSlopeAngle >= 0f;
                    float absFsAngle = isPlus ? Current.forwardSlopeAngle : -Current.forwardSlopeAngle;
                    float accel = MoveOption.slopeAccel * absFsAngle * 0.01111f + 1f;
                    Current.slopeAccel = !isPlus ? accel : 1.0f / accel;

                    Current.horizontalVelocity *= Current.slopeAccel;
                }

                // Fix Here
                // 벡터 회전 (경사로)
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

