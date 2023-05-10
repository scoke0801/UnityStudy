using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

namespace _Monster
{
    // 대기 상태
    public class IdleState : State
    {
        private readonly StatefulObject owner;

        public IdleState(StatefulObject owner)
        {
            this.owner = owner;
        }

        public override void Enter()
        {
            Debug.Log("IdleState: Enter");
        }

        public override void Update()
        {
            Debug.Log("IdleState: Update");
        }

        public override void Exit()
        {
            Debug.Log("IdleState: Exit");
        }
    }

    // 경계 상태
    public class PatrolState : State
    {
        private readonly StatefulObject owner;

        public PatrolState(StatefulObject owner)
        {
            this.owner = owner;
        }

        public override void Enter()
        {
            Debug.Log("PatrolState: Enter");        }

        public override void Update()
        {
            Debug.Log("PatrolState: Update");
        }

        public override void Exit()
        {
            Debug.Log("PatrolState: Exit");
        }
    }

    // 탐색 상태
    public class ChaseState : State
    {
        private readonly StatefulObject owner;

        public ChaseState(StatefulObject owner)
        {
            this.owner = owner;
        }

        public override void Enter()
        {
            Debug.Log("ChaseState: Enter");
        }

        public override void Update()
        {
            Debug.Log("ChaseState: Update");
        }

        public override void Exit()
        {
            Debug.Log("ChaseState: Exit");
        }
    }

    // 추적 상태
    public class TraceState : State
    {
        private readonly StatefulObject owner;

        public TraceState(StatefulObject owner)
        {
            this.owner = owner;
        }

        public override void Enter()
        {
            Debug.Log("TrackState: Enter");
        }

        public override void Update()
        {
            Debug.Log("TrackState: Update");
        }
        public override void Exit()
        {
            Debug.Log("TrackState: Exit");
        }
    }

    // 공격 상태
    public class AttackState : State
    {
        private readonly StatefulObject owner;
        public AttackState(StatefulObject owner)
        {
            this.owner = owner;
        }

        public override void Enter()
        {
            Debug.Log("AttackState: Enter");
        }

        public override void Update()
        {
            Debug.Log("AttackState: Update");

            // 공격 로직
        }

        public override void Exit()
        {
            Debug.Log("AttackState: Exit");
        }
    }
}