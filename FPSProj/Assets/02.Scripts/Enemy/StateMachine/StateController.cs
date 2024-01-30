using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
                if(statDict == null) { return null; }
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

        public bool Strafing
        {
            get => strafing;
            set
            {
                enemyAnimation.anim.SetBool("Strafe", value);
                strafing = true;
            }
        }

        public bool Aiming
        {
            get => aiming;
            set
            {
                if(aiming != value)
                {
                    enemyAnimation.anim.SetBool("Aim", value);
                    aiming = value;
                }
            }
        }

        public IEnumerator UnstuckAim(float delay)
        {
            // �ִϸ��̼� �߰��� �����̸� �ֱ� ���� �뵵�� ���.
            yield return new WaitForSeconds(delay * 0.5f);

            Aiming = false;

            yield return new WaitForSeconds(delay * 0.5f);

            Aiming = true;
        }

        private void Awake()
        {
            if(coverSpot == null)
            {
                coverSpot = new Dictionary<int, Vector3>();
            }

            // �������� ���� ���� positiveInfinity...
            coverSpot[GetHashCode()] = Vector3.positiveInfinity;

            navAgent = GetComponent<NavMeshAgent>();

            aiActive = true;
            enemyAnimation = gameObject.AddComponent<EnemyAnimation>();
            magBullets = bullets;

            variables.shotsinRounds = maximumBurst;

            nearRadius = perceptionRadius * 0.5f;

            GameObject gameController = GameObject.FindGameObjectWithTag(Defs.TagAndLayer.TagName.GameController);
            coverLookup = gameController.GetComponent<CoverLookUp>();
            if(coverLookup == null)
            {
                coverLookup = gameController.AddComponent<CoverLookUp>();
                coverLookup.Setup(generalStats.coverMask);
            }

            Debug.Assert(aimTarget.root.GetComponent<HealthBase>(), "�ݵ�� Ÿ�ٿ��� ����� ���� ������Ʈ�� �־�� �մϴ�.");
        }

        public void Start()
        {
            currentState.OnEnableActions(this);
        }

        private void Update()
        {
            checkedOnLoop = false;

            if (!aiActive)
            {
                return;
            }

            /// ���� ������Ʈ�鿡 ���ؼ� �׼��� ����.
            currentState.DoActions(this);
            currentState.CheckTransition(this);
        }

        private void OnDrawGizmos()
        {
            if(currentState != null)
            {
                Gizmos.color = currentState.sceneGizmoColor;
                Gizmos.DrawWireSphere(transform.position + Vector3.up * 2.5f, 2f);
            }
        }

        public void EndReloadWeapon()
        {
            reloading = false;
            bullets = magBullets;
        }

        public void AlertCallback(Vector3 target)
        {
            if (!aimTarget.root.GetComponent<HealthBase>().IsDead)
            {
                variables.heartAlert = true;
                personalTarget = target;
            }
        }

        public bool IsNearOtherSpot(Vector3 spot, float margin = 1f)
        {
            foreach( KeyValuePair<int, Vector3> usedSpot in coverSpot)
            {
                if(usedSpot.Key != gameObject.GetHashCode() && Vector3.Distance(spot, usedSpot.Value) <= margin)
                {
                    return true;
                }
            }

            return false;
        }

        public bool BlockedSight()
        {
            if (!checkedOnLoop)
            {
                checkedOnLoop = true;

                Vector3 target = default;

                try
                {
                    target = aimTarget.position;
                }
                catch (UnassignedReferenceException)
                {
                    Debug.LogError($"���� Ÿ���� �������ּ���. + {transform.name}");
                }

                Vector3 castOrigin = transform.position + Vector3.up * generalStats.aboveCoverHeight;
                Vector3 dirToTarget = target - castOrigin;

                // �þ߸� ���� ����� �����ϴ� �� �˻�.
                blockedSight = Physics.Raycast(castOrigin, dirToTarget, out RaycastHit hit,
                    dirToTarget.magnitude,
                    generalStats.coverMask | generalStats.obstacleMask);
            }

            return blockedSight;
        }

        private void OnDestroy()
        {
            coverSpot.Remove(GetHashCode());
        }
    }


}