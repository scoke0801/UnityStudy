using System.Collections;
using System.Collections.Generic;
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
    }


}