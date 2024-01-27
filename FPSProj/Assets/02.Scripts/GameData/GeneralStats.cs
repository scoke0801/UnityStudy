using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="PluggableAI/GeneralStats")]
public class GeneralStats : ScriptableObject
{
    [Header("Gerneral")]
    [Tooltip("NPC ���� �ӵ�(����)")]
    public float patrolSpeed = 2f;

    [Tooltip("NPC ������� �ӵ�")]
    public float chaseSpeed =5f;

    [Tooltip("NPC ȸ���ϴ� �ӵ�")]
    public float evadeSpeed = 15f;

    [Tooltip("NPC ��������Ʈ���� ����ϴ� �ð�")]
    public float patrolWaitTime = 2f;

    [Header("Animation")]
    [Tooltip("��ֹ� ���̾� ����ũ")]
    public LayerMask obstacleMask;

    [Tooltip("���� �� �������� ���ϱ� ���� �ּ� Ȯ�� �ޱ�")]
    public float angleDeadZone = 5f;

    [Tooltip("�ӵ� ���� �ð�")]
    public float speedDampTime = 0.4f;

    [Tooltip("���ӵ� ���� �ð�")]
    public float angularSpeedDampTime = 0.2f;

    [Tooltip("���ӵ� �ȿ��� ���� ȸ���� ���� ���� �ð�")]
    public float angleReasponseTime = 0.2f;

    [Header("Cover")]
    [Tooltip("��ֹ��� ������ �� ����ؾ��� �ּ� ���� ��")]
    public float aboveCoverHeight = 1.5f;

    [Tooltip("��ֹ� ���̾� ����ũ")]
    public LayerMask coverMask;

    [Tooltip("��� ���̾� ����ũ")]
    public LayerMask shotMask;  // ������ �� �� �ִ� ���

    [Tooltip("Ÿ�� ���̾� ����ũ")]
    public LayerMask targetMask; // ���� ���
}
