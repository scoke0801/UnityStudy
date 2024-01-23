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
    private Transform myTransform;
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

    private int groundedBool;  // �ִϸ����� ���� �ִ°�.
    private Vector3 colisionExtents; // ������ �浹üũ�� ���� �浹ü ����.

    public float GetH { get => horizontal; }
    public float GetV { get => vertical; }
    public ThirdPirsonOrbitCamera GetCamScript { get => cameraScript; }

    public Rigidbody GetRigidbody { get => myRigidbody; }
    public Animator GetAnimator { get => myAnimator; }

    public int GetDefaultBehavior { get => defaultBehavior; }

    private void Awake()
    {
        behaviors = new List<GenericBehavior>();
        overrideBehaviors = new List<GenericBehavior>();
        myAnimator = GetComponent<Animator>();
        hFloat = Animator.StringToHash(Defs.AnimatorKey.Horizontal);
        vFloat = Animator.StringToHash(Defs.AnimatorKey.Vertical);

        myTransform = transform;
        cameraScript = playerCamera.GetComponent<ThirdPirsonOrbitCamera>();
        myRigidbody = GetComponent<Rigidbody>();

        // ground?
        groundedBool = Animator.StringToHash(Defs.AnimatorKey.Grounded);
        colisionExtents = GetComponent<Collider>().bounds.extents;
    }

    public bool IsMoving()
    {
        return Mathf.Abs(horizontal) > Mathf.Epsilon || Mathf.Abs(vertical) > Mathf.Epsilon;
    }

    public bool IsHorizontalMoving()
    {
        return Mathf.Abs(horizontal) > Mathf.Epsilon;
    }

    public bool CanSprint()
    {
        foreach(GenericBehavior behavior in behaviors)
        {
            if(!behavior.AllowSprint)
            {
                return false;
            }
        }
        foreach (GenericBehavior genericBehavior in overrideBehaviors)
        {
            if(!genericBehavior.AllowSprint)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsSprinting()
    {
        return sprint && IsMoving() && CanSprint();
    }

    public bool IsGrounded()
    {
        Ray ray = new Ray(myTransform.position + Vector3.up * 2 * colisionExtents.x, Vector3.down);
        return Physics.SphereCast(ray, colisionExtents.x, colisionExtents.x + 0.2f);
    }

    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        myAnimator.SetFloat(hFloat, horizontal, 0.1f, Time.deltaTime);
        myAnimator.SetFloat(vFloat, vertical, 0.1f, Time.deltaTime);

        sprint = Input.GetButton(ButtonName.Sprint);
        if(IsSprinting())
        {
            changedFOV = true;
            cameraScript.SetFOV(sprintFOV);
        }
        else if (changedFOV)
        {
            cameraScript.ResetFOV();
            changedFOV = false;
        }

        myAnimator.SetBool(groundedBool, IsGrounded());
    }

    public void Repositioning()
    {
        if(lastDirection != Vector3.zero)
        {
            lastDirection.y = 0.0f;

            Quaternion targetRotation = Quaternion.LookRotation(lastDirection);
            Quaternion newRotation = Quaternion.Slerp(myRigidbody.rotation, targetRotation, turnSmoothing);

            myRigidbody.MoveRotation(newRotation);
        }
    }

    private void FixedUpdate()
    {
        bool isAnyBehaviorActive = false;
        if(lockedBahavior > 0 || overrideBehaviors.Count == 0)
        { 
            foreach (GenericBehavior behavior in behaviors)
            {
                if(behavior.isActiveAndEnabled && currentBahavior == behavior.GetBehaviorCode)
                {
                    isAnyBehaviorActive = true;
                    behavior.LocalFixedUpdate();
                }
            }
        }
        else
        {
            foreach(GenericBehavior behavior in overrideBehaviors)
            {
                behavior.LocalFixedUpdate();
            }
        }

        if(!isAnyBehaviorActive && overrideBehaviors.Count == 0)
        {
            myRigidbody.useGravity = true;
            Repositioning();
        }
    }

    private void LateUpdate()
    {
        if (lockedBahavior > 0 || overrideBehaviors.Count == 0)
        {
            foreach(GenericBehavior behavior in behaviors)
            {
                if(behavior.isActiveAndEnabled && currentBahavior == behavior.GetBehaviorCode)
                {
                    behavior.LocalLateUpdate();
                }
            }
        }
    }

    public void SubscribeBehavior(GenericBehavior behavior)
    {
        behaviors.Add(behavior);
    }

    public void RegisterDefaultBehavior(int behaviorCode)
    {
        defaultBehavior = behaviorCode;
        currentBahavior = behaviorCode;
    }

    public void RegisterBehavior(int behaviorCode)
    {
        if(currentBahavior == defaultBehavior)
        {
            currentBahavior = behaviorCode;
        }
    }

    public void UnRegisterBehavior(int behaviorCode)
    {
        if(currentBahavior == behaviorCode)
        {
            currentBahavior = defaultBehavior;
        }
    }

    public bool OverrideWithBehavior(GenericBehavior behavior)
    {
        if(!overrideBehaviors.Contains(behavior))
        {
            if(overrideBehaviors.Count == 0)
            {
                foreach(GenericBehavior behavior1 in behaviors)
                {
                    if(behavior1.isActiveAndEnabled &&
                        currentBahavior == behavior1.GetBehaviorCode)
                    {
                        behavior1.OnOverride();
                        break;
                    }
                }
            }

            overrideBehaviors.Add(behavior);
            return true;
        }
        return false;
    }

    public bool RevokeOverridingBehavior(GenericBehavior behavior)
    {
        if(overrideBehaviors.Contains(behavior))
        {
            overrideBehaviors.Remove(behavior);
            return true;
        }

        return false;
    }

    public bool IsOverriding(GenericBehavior behavior = null)
    {
        if(behavior == null)
        {
            return overrideBehaviors.Count > 0;
        }

        return overrideBehaviors.Contains(behavior);
    }

    public bool IsCurrentBehavior(int behaviorCode)
    {
        return currentBahavior == behaviorCode;
    }

    public bool GetTempLockStatus(int behaviorCode = 0)
    {
        return (lockedBahavior != 0 && lockedBahavior != behaviorCode);
    }

    public void LockTempBehavior(int behaviorCode)
    {
        if(behaviorCode == 0)
        {
            lockedBahavior = behaviorCode;
        }
    }

    public void UnLockTempBehavior(int behaviorCode)
    {
        if(lockedBahavior == behaviorCode)
        {
            lockedBahavior = 0;
        }
    }

    public Vector3 GetLastDirection() { return lastDirection; }

    public void SetLastDirection(Vector3 direction) { lastDirection = direction; }
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