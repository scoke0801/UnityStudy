using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Monster
{
    public abstract class State
    {
        // ���°� ���۵� �� ȣ��Ǵ� �޼���
        public virtual void Enter() { }

        // ���°� ����� �� ȣ��Ǵ� �޼���
        public virtual void Exit() { }

        // �� �����Ӹ��� ȣ��Ǵ� �޼���
        public abstract void Update();
    }

    // �ൿ Ŭ����
    public abstract class Action
    {
        // �ൿ�� ������ �� ȣ��Ǵ� �޼���
        public abstract void Execute();
    }

    // ���¸� ������ ��ü Ŭ����
    public class StatefulObject : MonoBehaviour
    {
        private State currentState;
        public State CurretState { get { return currentState; } }

        private Dictionary<string, State> states = new Dictionary<string, State>();
        private Dictionary<string, Action> actions = new Dictionary<string, Action>();

        // ���¸� �߰��ϴ� �޼���
        public void AddState(string name, State state)
        {
            states[name] = state;
        }

        // �ൿ�� �߰��ϴ� �޼���
        public void AddAction(string name, Action action)
        {
            actions[name] = action;
        }

        // ���¸� �����ϴ� �޼���
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

        // �� �����Ӹ��� ȣ��Ǵ� �޼���
        private void Update()
        {
            if (currentState != null)
            {
                currentState.Update();
            }
        }

        // �ൿ�� �����ϴ� �޼���
        public void DoAction(string name)
        {
            if (actions.ContainsKey(name))
            {
                actions[name].Execute();
            }
        }
    }
}
