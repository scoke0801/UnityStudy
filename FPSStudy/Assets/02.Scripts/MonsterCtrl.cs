using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// �ൿ �ó�����.
// �ұ�Ģ������ ����.
// ���ΰ� ĳ���Ϳ� �����ϸ� ������ ����
// ���� �����Ÿ��� ������ ���ΰ��� ���� ����.

public class MonsterCtrl : MonoBehaviour
{
    public enum State
    {
        IDLE,
        TRACE,
        ATTACK,
        DIE,
    }
    public State state = State.IDLE;

    public float traceDist = 10.0f;
    public float attackDist = 2.0f;
    public bool isDie = false;

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator anim;

    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");

    // Start is called before the first frame update
    void Start()
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();

        agent = GetComponent<NavMeshAgent>();

        anim = GetComponent<Animator>();

        StartCoroutine(CheckMonsterState());

        StartCoroutine(MonsterAction());
    }

    IEnumerator CheckMonsterState()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.3f);
        while (!isDie)
        {
            yield return waitForSeconds;

            float distance = Vector3.Distance(playerTr.position, monsterTr.position);

            if (distance <= attackDist)
            {
                state = State.ATTACK;
            }
            else if(distance <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.IDLE;
            }
        }
    }

    IEnumerator MonsterAction()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(0.3f);
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    agent.isStopped = true;
                    anim.SetBool(hashTrace, false);
                    break;
                case State.TRACE:
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    anim.SetBool(hashTrace, true);
                    anim.SetBool(hashAttack, false);
                    break;
                case State.ATTACK:
                    anim.SetBool(hashAttack, true);
                    break;
                case State.DIE:
                    break;
            }

            yield return waitForSeconds;
        }
    }

    private void OnDrawGizmos()
    {
        if (state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDist);
        }

        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDist);
        }
    }
}
