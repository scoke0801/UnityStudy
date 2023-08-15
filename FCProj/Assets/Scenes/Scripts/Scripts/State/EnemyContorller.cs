using UnityEditor;
using UnityEngine;

public class EnemyContorller : MonoBehaviour
{
    StateMachineEx<EnemyContorller> _stateMachine;

    private void Start()
    {
        _stateMachine = new StateMachineEx<EnemyContorller>(this, new IdleState());

        _stateMachine.AddState(new MoveState());
        _stateMachine.AddState(new AttackState());
    }

    private void Update()
    {
        _stateMachine.Update(Time.deltaTime);
    }


}
