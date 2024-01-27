using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    /// <summary>
    /// state -> actions update -> transition (decision) check...
    /// state�� �ʿ��� ��ɵ�.
    /// �ִϸ��̼� �ݹ��.
    /// �þ� üũ.
    /// ã�� ���� ��� �� ���� ����� ��ġ Ž��
    /// </summary>
    public class StateController : MonoBehaviour
    {
        private static Dictionary<int, Vector3> coverSpot;

        public ClassStats statData;
        public GeneralStats generalStats;
        public string classID; // PISTOL, RIFLE, AK

        private Dictionary<string, ClassStats.Param> statDict;
        public ClassStats.Param classStats
        { 
            get
            {
                if( statDict.TryGetValue(classID, out ClassStats.Param ret) )
                {
                    return ret;
                }

                foreach(ClassStats.Sheet sheet in statData.sheets)
                {
                    foreach(ClassStats.Param param in sheet.list)
                    {
                        if (param.ID.Equals(classID))
                        {
                            statDict.Add(classID, param);
                            return param;
                        }
                    }
                }
                return null;
            }
        }
        public State currentState;
        public State remainState;

        public Transform aimTarget;

        public List<Transform> patrolWayPoints;

        public int bullets;

        // �þ� ���� ����
        [Range(0, 50)] public float viewRadius;        // ��ü�� �� �� �ִ� �þ� �ݰ�.
        [Range(0, 360)] public float viewAngle;         // ��ü�� �� �� �ִ� ����
        [Range(0,25)] public float perceptionRadius;     // viewAngle�� �� �� ū ����

        [HideInInspector] public float nearRadius;    // ����� �ݰ�.
        [HideInInspector] public NavMeshAgent navAgent;
        [HideInInspector] public int wayPointIndex;
        [HideInInspector] public int maximumBurst = 7;    // ��ȿ �Ѿ� ����

        [HideInInspector] public float blindEngageTime = 30f; // �÷��̾ ���� �ν��� �����ϰ� ���� �ð�.

        [HideInInspector]public bool targetInSight;  // Ÿ���� �þ߿� �ִ���
        [HideInInspector]public bool focusSight;     // Ÿ���� ��Ŀ�� �� ��
        [HideInInspector]public bool reloading;
        [HideInInspector]public bool hadClearShot;       // ���� �߻簡�� ����
        [HideInInspector] public bool haveClearShot;      // ���� �߻簡�� ����

        [HideInInspector] public int coverHash = -1;      //

        [HideInInspector] public EnemyVariables variables;        //
        [HideInInspector] public Vector3 personalTarget = Vector3.zero;   // �ൿ�� ���� ���.

        private int magBullets; // ���� ��ź
        private bool aiActive; // Ȱ��ȭ ����
        private bool strafing;  // ȸ���� ����
        private bool aiming;
        private bool checkedOnLoop; // loop���� Ȯ�� ����
        private bool blockedSight;  // �þ߰� ������ ��

        [HideInInspector] public EnemyAnimation enemyAnimation;
        [HideInInspector] public CoverLookUp coverLookup;

        public Vector3 CoverSpot
        {
            get { return coverSpot[GetHashCode()]; }
            set { coverSpot[GetHashCode()] = value; }
        }

        public void TransitionToState(State nextState, Decision decision)
        {
            if(nextState != remainState)
            {
                currentState = nextState;
            }
        }
    }


}