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

        // �ʱ� ���� ����
        ChangeState("Idle");
    }

    void Update()
    {
        // ���� ���� ������Ʈ
        CurretState.Update();
    }
}
