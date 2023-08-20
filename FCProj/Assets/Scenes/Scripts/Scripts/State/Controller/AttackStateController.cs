using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateController : MonoBehaviour
{
    public delegate void OnEnterAttackState();
    public delegate void OnExitAttackState();

    public OnEnterAttackState _enterAttackStateHandelr;
    public OnExitAttackState _exitAttackStateHandler;

    public bool IsInAttackState
    {
        get;
        private set;
    }

    // Start is called before the first frame update
    void Start()
    {
        _enterAttackStateHandelr = new OnEnterAttackState(EnterAttackState);

        _exitAttackStateHandler = new OnExitAttackState(ExitAttackState);
    }

    #region Callback Methods
    public void OnStartOfAttackState()
    {
        IsInAttackState = true;
        _enterAttackStateHandelr();
    }
    public void OnEndOfAttackState()
    {
        IsInAttackState = false;
        _exitAttackStateHandler();
    }

    public void EnterAttackState()
    {

    }

    public void ExitAttackState()
    {

    }

    public void OnCheckAttackController(int attackIndex)
    {
        // 해당 인터페이스를 구현한 오브젝트만 가능.
        GetComponent<IAttackable>()?.OnExecuteAttack(attackIndex);
    }
    #endregion
}
