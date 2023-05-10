using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Monster
{
    public abstract class State
    {
        // 상태가 시작될 때 호출되는 메서드
        public virtual void Enter() { }

        // 상태가 종료될 때 호출되는 메서드
        public virtual void Exit() { }

        // 매 프레임마다 호출되는 메서드
        public abstract void Update();
    }

    // 행동 클래스
    public abstract class Action
    {
        // 행동을 실행할 때 호출되는 메서드
        public abstract void Execute();
    }

    // 상태를 가지는 객체 클래스
    public class StatefulObject : MonoBehaviour
    {
        private State currentState;
        public State CurretState { get { return currentState; } }

        private Dictionary<string, State> states = new Dictionary<string, State>();
        private Dictionary<string, Action> actions = new Dictionary<string, Action>();

        // 상태를 추가하는 메서드
        public void AddState(string name, State state)
        {
            states[name] = state;
        }

        // 행동을 추가하는 메서드
        public void AddAction(string name, Action action)
        {
            actions[name] = action;
        }

        // 상태를 변경하는 메서드
        public void ChangeState(string name)
        {
            if (currentState != null)
            {
                currentState.Exit();
            }

            currentState = states[name];

            if (currentState != null)
            {
                currentState.Enter();
            }
        }

        // 매 프레임마다 호출되는 메서드
        private void Update()
        {
            if (currentState != null)
            {
                currentState.Update();
            }
        }

        // 행동을 실행하는 메서드
        public void DoAction(string name)
        {
            if (actions.ContainsKey(name))
            {
                actions[name].Execute();
            }
        }
    }
}
