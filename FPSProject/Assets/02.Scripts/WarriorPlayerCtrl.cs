using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

#pragma warning disable IDE0051

public class WarriorPlayerCtrl : MonoBehaviour
{
    private Animator anim;
    private new Transform transform;
    private Vector3 moveDir;

    private PlayerInput playerInput;
    private InputActionMap mainActionMap;
    private InputAction moveAction;
    private InputAction attackAction;

    private void Start()
    {
        anim = GetComponent<Animator>();
        transform = GetComponent<Transform>();
        playerInput = GetComponent<PlayerInput>();

        mainActionMap = playerInput.actions.FindActionMap("PlayerActions");

        moveAction = mainActionMap.FindAction("Move");
        attackAction = mainActionMap.FindAction("Attack");

        moveAction.performed += context =>
        {
            Vector2 dir = context.ReadValue<Vector2>();
            moveDir = new Vector3(dir.x, 0, dir.y);
            anim.SetFloat("Movement", dir.magnitude);
        };

        moveAction.canceled += context =>
        {
            moveDir = Vector3.zero;
            anim.SetFloat("Movement", 0.0f);
        };

        attackAction.performed += context =>
        {
            Debug.Log("attack By C# event");
            anim.SetTrigger("Attack");
        };
    }

    private void Update()
    {
        if (moveDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDir);
            transform.Translate(Vector3.forward * Time.deltaTime * 4.0f);
        }
    }
    #region SEND_MESSAGE
    void OnMove(InputValue value)
    {
        Vector2 dir = value.Get<Vector2>();
        moveDir = new Vector3(dir.x, 0, dir.y);

        anim.SetFloat("Movement", dir.magnitude);

        Debug.Log($"Move = ({dir.x}, {dir.y})");
    }

    void OnAttack()
    {
        anim.SetTrigger("Attack");
        Debug.Log("Attack");
    }
    #endregion

    #region INVOKE_UNITY_EVENTS
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();

        moveDir = new Vector3(dir.x, 0, dir.y);

        anim.SetFloat("Movement", dir.magnitude);
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log($"context.phase={context.phase}");

        if(context.performed)
        {
            Debug.Log("Attack");
            anim.SetTrigger("Attack");
        }
    }
    #endregion

}
