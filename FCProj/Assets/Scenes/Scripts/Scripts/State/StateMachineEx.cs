using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace State
{
    public abstract class State<T>
    {
        protected StateMachineEx<T> stateMachine;
        protected T context;        // 해당 상태의 소유자에 대한 context

        public State()
        {
        }

        internal void SetStateMachineAndContext(StateMachineEx<T> stateMachine, T context)
        {
            this.stateMachine = stateMachine;
            this.context = context;

            OnInitialized();
        }

        public virtual void OnInitialized(){}
        public virtual void OnEnter() { }
        public virtual void OnExit() {}
        public virtual void Update(float deltaTime) { }
    }

    public sealed class StateMachineEx<T>
    { 
        private T _context;
        private State<T> _currentState;

        public State<T> CurrentState => _currentState;

        private State<T> _priviousState;
        public State<T> PriviousState => _priviousState;

        private float _elapsedTimeInState = 0.0f;
        public float ElpasedTimeInState => _elapsedTimeInState;

        private Dictionary<System.Type, State<T>> _states = new Dictionary<System.Type, State<T>>();

        public StateMachineEx(T context, State<T> initialState)
        {
            this._context = context;

            // 초기 상태 설정.
            AddState(initialState);
            _currentState = initialState;
            _currentState.OnEnter();
        }

        public void AddState(State<T> state)
        {
            state.SetStateMachineAndContext(this, this._context);
            _states[state.GetType()] = state;
        }

        public void Update(float deltaTime)
        {
            _elapsedTimeInState += deltaTime;

            _currentState.Update(deltaTime);
        }

        public R ChangeState<R>() where R : State<T>
        {
            var newType = typeof(R);
            if (_currentState.GetType() == newType)
            {
                return _currentState as R;
            }

            if (_currentState != null)
            {
                _currentState.OnExit();
            }

            _priviousState = CurrentState;
            _currentState = _states[newType];
            _currentState.OnEnter();

            _elapsedTimeInState = 0.0f;

            return _currentState as R;
        }

    }
}
