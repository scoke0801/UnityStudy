using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    /// <summary>
    /// state -> actions update -> transition (decision) check...
    /// state에 필요한 기능들.
    /// 애니메이션 콜백들.
    /// 시야 체크.
    /// 찾은 엄폐물 장소 중 가장 가까운 위치 탐색
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

        // 시야 관련 변수
        [Range(0, 50)] public float viewRadius;        // 객체가 볼 수 있는 시야 반경.
        [Range(0, 360)] public float viewAngle;         // 객체가 볼 수 있는 각도
        [Range(0,25)] public float perceptionRadius;     // viewAngle의 좀 더 큰 버전

        [HideInInspector] public float nearRadius;    // 가까운 반경.
        [HideInInspector] public NavMeshAgent navAgent;
        [HideInInspector] public int wayPointIndex;
        [HideInInspector] public int maximumBurst = 7;    // 유효 총알 개수

        [HideInInspector] public float blindEngageTime = 30f; // 플레이어에 대한 인식을 유지하고 있을 시간.

        [HideInInspector]public bool targetInSight;  // 타겟이 시야에 있는지
        [HideInInspector]public bool focusSight;     // 타겟을 포커싱 할 지
        [HideInInspector]public bool reloading;
        [HideInInspector]public bool hadClearShot;       // 과거 발사가능 여부
        [HideInInspector] public bool haveClearShot;      // 현재 발사가능 여부

        [HideInInspector] public int coverHash = -1;      //

        [HideInInspector] public EnemyVariables variables;        //
        [HideInInspector] public Vector3 personalTarget = Vector3.zero;   // 행동에 따른 대상.

        private int magBullets; // 현재 잔탄
        private bool aiActive; // 활성화 여부
        private bool strafing;  // 회피중 여부
        private bool aiming;
        private bool checkedOnLoop; // loop에서 확인 여부
        private bool blockedSight;  // 시야가 막혔는 지

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
            // 애니메이션 중간에 딜레이를 주기 위한 용도로 사용.
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

            // 설정되지 않은 경우는 positiveInfinity...
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

            Debug.Assert(aimTarget.root.GetComponent<HealthBase>(), "반드시 타겟에는 생명력 관련 컴포넌트가 있어야 합니다.");
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

            /// 현재 스테이트들에 대해서 액션을 실행.
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
                    Debug.LogError($"조준 타겟을 지정해주세요. + {transform.name}");
                }

                Vector3 castOrigin = transform.position + Vector3.up * generalStats.aboveCoverHeight;
                Vector3 dirToTarget = target - castOrigin;

                // 시야를 막는 대상이 존재하는 지 검사.
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