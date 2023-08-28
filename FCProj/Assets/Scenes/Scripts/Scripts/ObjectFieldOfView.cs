using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFieldOfView : MonoBehaviour
{
    [SerializeField] private float _viewRadius = 5f;

    [Range(0, 360), SerializeField] private float _viewAngle = 90.0f;

    [SerializeField] private float _delayToSearch = 0.2f;

    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstacleMask;

    private List<Transform> _visibleTargets = new List<Transform>();
    private Transform _nearestTarget;

    private float _distanceToTarget = 0.0f;

    public float ViewRadius => _viewRadius;
    public float ViewAngle => _viewAngle;

    public List<Transform> VisibleTargets => _visibleTargets;
    public float DistanceToTarget => _distanceToTarget;
    public Transform NearestTarget => _nearestTarget;
    public LayerMask TargetMask => _targetMask;

    private void Start()
    {
        StartCoroutine(nameof(FindTargetsWithDelay), _delayToSearch);
    }

    private void Update()
    {
        FindVisibleTargets();
    }

    void FindVisibleTargets()
    {
        _distanceToTarget = 0.0f;
        _nearestTarget = null;
        _visibleTargets.Clear();

        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _targetMask);

        for(int i = 0; i < targetInViewRadius.Length; ++i)
        {
            Transform target = targetInViewRadius[i].transform;

            Vector3 dirToTarget = (target.position - transform.position).normalized;
            // 시야각에 대상이 존재하는 지
            if(Vector3.Angle(transform.forward, dirToTarget) < _viewAngle * 0.5f)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                // 장애물이 가리고 있는지
                if(!Physics.Raycast(transform.position, dirToTarget, distToTarget, _obstacleMask))
                {
                    _visibleTargets.Add(target);

                    //  가장 가까운 대상인지 계산.
                    if(_nearestTarget == null || (_distanceToTarget > distToTarget))
                    {
                        _nearestTarget = target;
                    }

                    _distanceToTarget = distToTarget;
                }
            }
        }
    }


    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

}
