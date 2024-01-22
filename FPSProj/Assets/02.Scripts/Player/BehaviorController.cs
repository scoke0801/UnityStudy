using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ����, �⺻ ����, �������̵� ����, ��� ����, ���콺 �̵���.
/// ���� ���ִ���, GenericBehaviour�� ��ӹ��� ���۵��� ������Ʈ �����ݴϴ�.
/// </summary>
public class BehaviorController : MonoBehaviour
{
    private List<GenericBehavior> behaviors;         // ���۵�
    private List<GenericBehavior> overrideBehaviors; // �켱 �� �Ǵ� ����.


    private int currentBahavior; // ���� �⺻ ���� �ؽ��ڵ�.
    private int defaultBehavior; // �⺻ ���� �ؽ��ڵ�.
    private int lockedBahavior;  // ��� ���� �ؽ��ڵ�.

    // ĳ��.
    public Transform playerCamera;
    private Animator myAnimator;
    private Rigidbody myRigidbody;
    private ThirdPirsonOrbitCamera cameraScript;

    // 
    private float horizontal;   // horizontal axis input;
    private float vertical;     // vertical axis input;
    public float turnSmoothing = 0.06f; // ī�޶� ���ϵ��� ������ �� ȸ�� �ӵ�.
    private bool changedFOV;    // �޸��� ������ ī�޶� �þ߰��� ����Ǿ��� �� ����Ǿ����� ����.
    public float sprintFOV = 100;   // �޸��� �þ߰�.
    private Vector3 lastDirection;  // ���������� ���ϰ� �ִ� ����.

    private bool sprint;        // �޸��� ���ΰ�?

    private int hFloat;   // �ִϸ����� ���� ������ ��.
    private int vFloat;   // �ִϸ����� ���� ������ ��.

    private bool groundedBool;  // �ִϸ����� ���� �ִ°�.
    private Vector3 colisionExtents; // ������ �浹üũ�� ���� �浹ü ����.

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