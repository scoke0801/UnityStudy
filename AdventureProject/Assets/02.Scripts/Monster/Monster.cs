using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using _Monster;
using Unity.VisualScripting;

public class Monster : StatefulObject
{
    void Start()
    {
        AddState("Idle", new IdleState(this) );

        // 초기 상태 설정
        ChangeState("Idle");
    }

    void Update()
    {
        // 현재 상태 업데이트
        CurretState.Update();
    }
}
