using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 현재 동작, 기본 동작, 오버라이딩 동작, 잠긴 동작, 마우스 이동값.
/// 땅에 서있는지, GenericBehaviour를 상속받은 동작들을 업데이트 시켜줍니다.
/// </summary>
public class BehaviorController : MonoBehaviour
{
    private List<GenericBehavior> behaviors;         // 동작들
    private List<GenericBehavior> overrideBehaviors; // 우선 시 되는 동작.


    private int currentBahavior; // 현재 기본 동작 해시코드.
    private int defaultBehavior; // 기본 동작 해시코드.
    private int lockedBahavior;  // 잠긴 동작 해시코드.

    // 캐싱.
    public Transform playerCamera;
    private Animator myAnimator;
    private Rigidbody myRigidbody;
    private ThirdPirsonOrbitCamera cameraScript;

    // 
    private float horizontal;   // horizontal axis input;
    private float vertical;     // vertical axis input;
    public float turnSmoothing = 0.06f; // 카메라를 향하도록 움직일 때 회전 속도.
    private bool changedFOV;    // 달리기 동작이 카메라 시야각이 변경되었을 때 저장되었는지 여부.
    public float sprintFOV = 100;   // 달리기 시야각.
    private Vector3 lastDirection;  // 마지막으로 향하고 있던 방향.

    private bool sprint;        // 달리기 중인가?

    private int hFloat;   // 애니메이터 관련 가로축 값.
    private int vFloat;   // 애니메이터 관련 세로축 값.

    private bool groundedBool;  // 애니메이터 지상에 있는가.
    private Vector3 colisionExtents; // 땅과의 충돌체크를 위한 충돌체 영역.

    public float GetH { get => horizontal; }
    public float GetV { get => vertical; }
    public ThirdPirsonOrbitCamera GetCamScript { get => cameraScript; }

    public Rigidbody GetRigidbody { get => myRigidbody; }
    public Animator GetAnimator { get => myAnimator; }

    public int GetDefaultBehavior { get => defaultBehavior; }
}


public abstract class GenericBehavior : MonoBehaviour
{
    protected int speedFloat;
    protected BehaviorController behaviorController;

    protected int behaviorHashCode;
    protected bool canSprint;

    private void Awake()
    {
        behaviorController = GetComponent<BehaviorController>();
        speedFloat = Animator.StringToHash(Defs.AnimatorKey.Speed);
        canSprint = true;

        behaviorHashCode = GetType().GetHashCode();
    }

    public int GetBehaviorCode
    {
        get => behaviorHashCode;
    }

    public bool AllowSprint
    {
        get => canSprint;
    }

    public virtual void LocalLateUpdate()
    {

    }

    public virtual void LocalFixedUpdate()
    {

    }

    public virtual void OnOverride()
    {

    }
}