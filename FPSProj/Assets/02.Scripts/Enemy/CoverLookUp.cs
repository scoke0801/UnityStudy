using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    /// <summary>
    /// 숨을만한 곳을 찾아주는 컴포넌트. 
    /// 플레이어보다 멀리있는 건 제외.
    /// </summary>
    public class CoverLookUp : MonoBehaviour
    {
        private List<Vector3[]> allCoverSpots;
        private GameObject[] covers;
        private List<int> coverHashCodes; // cover target Unity ID

        private Dictionary<float, Vector3> filterSpots;

        private GameObject[] GetObjectsInLayerMask(int layerMask)
        {
            List<GameObject> ret = new List<GameObject>();

            foreach(GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (go.activeInHierarchy && layerMask == (layerMask | 1 << go.layer))
                {
                    ret.Add(go);
                }
            }

            return ret.ToArray();
        }

        private void ProcessPoint(List<Vector3> vecotor3s, Vector3 nativePoint ,float range)
        {
            NavMeshHit hit;
            if(NavMesh.SamplePosition(nativePoint, out hit, range, NavMesh.AllAreas))
            {
                vecotor3s.Add(hit.position);
            }
        }

        private Vector3[] GetSpots(GameObject go, LayerMask obstacleMask)
        {
            List<Vector3> bounds = new List<Vector3>();
            foreach(Collider collider in go.GetComponents<Collider>())
            {
                float baseHeight = (collider.bounds.center - collider.bounds.extents).y;
                float range = 2 * collider.bounds.extents.y;

                Vector3 desLocalForward = go.transform.forward * go.transform.localScale.z * 0.5f;
                Vector3 desLocalRight = go.transform.right * go.transform.localEulerAngles.x * 0.5f;

                if (go.GetComponent <MeshCollider>())
                {
                    float maxBounds = go.GetComponent<MeshCollider>().bounds.extents.z +
                        go.GetComponent<MeshCollider>().bounds.extents.x;
                    Vector3 originForward = collider.bounds.center + go.transform.forward * maxBounds;
                    Vector3 originRgiht = collider.bounds.center + go.transform.right * maxBounds;

                    if(Physics.Raycast(originForward, collider.bounds.center - originForward, out RaycastHit hit,
                        maxBounds, obstacleMask))
                    {
                        desLocalForward = hit.point - collider.bounds.center;
                    }

                    if(Physics.Raycast(originRgiht, collider.bounds.center - originRgiht, out hit, maxBounds,
                        obstacleMask))
                    {
                        desLocalRight = hit.point - collider.bounds.center;
                    }
                }
                else if(Vector3.Equals(go.transform.localScale, Vector3.one)) 
                {
                    desLocalForward = go.transform.forward * collider.bounds.extents.z;
                    desLocalRight = go.transform.right * collider.bounds.extents.z;
                }

                float edgeFactor = 0.75f;

                // 지점들에 대해 갈 수 있는 지 여부를 샘플링..
                ProcessPoint(bounds, collider.bounds.center + desLocalRight + desLocalForward * edgeFactor, range);
                ProcessPoint(bounds, collider.bounds.center + desLocalForward + desLocalRight * edgeFactor, range);
                ProcessPoint(bounds, collider.bounds.center + desLocalForward, range);
                ProcessPoint(bounds, collider.bounds.center + desLocalForward - desLocalRight * edgeFactor, range);
                ProcessPoint(bounds, collider.bounds.center - desLocalRight + desLocalForward * edgeFactor, range);
                ProcessPoint(bounds, collider.bounds.center + desLocalRight, range);
                ProcessPoint(bounds, collider.bounds.center + desLocalRight - desLocalForward * edgeFactor, range);
                ProcessPoint(bounds, collider.bounds.center - desLocalForward + desLocalRight * edgeFactor, range);
                ProcessPoint(bounds, collider.bounds.center - desLocalForward, range);
                ProcessPoint(bounds, collider.bounds.center - desLocalForward - desLocalRight * edgeFactor, range);
                ProcessPoint(bounds, collider.bounds.center - desLocalRight - desLocalForward * edgeFactor, range);
                ProcessPoint(bounds, collider.bounds.center - desLocalRight, range);
                
            }

            return bounds.ToArray();
        }

        public void Setup(LayerMask coverMask)
        {
            covers = GetObjectsInLayerMask(coverMask);

            coverHashCodes = new List<int>();
            allCoverSpots = new List<Vector3[]>();

            foreach( GameObject cover in covers)
            {
                allCoverSpots.Add(GetSpots(cover, coverMask));
                coverHashCodes.Add(cover.GetHashCode());
            }
        }


        // 목표물이 경로에 있는 지 확인. 대상이 각도 안에 있고 지점보다 가까이 있는지/
        private bool TargetInPath(Vector3 origin, Vector3 spot, Vector3 target, float angle)
        {
            Vector3 dirToTarget = (target - origin).normalized;
            Vector3 dirToSpot = (spot - origin).normalized;

            if(Vector3.Angle(dirToSpot, dirToTarget) <= angle)
            {
                float targetDist = (target - origin).sqrMagnitude;
                float spotDist = (spot - origin).sqrMagnitude;

                return (targetDist <= spotDist);
            }

            return false;
        }

        // 가장 가까운 유효한 지점을 탐색, 거리도 같이 output
        private ArrayList FilterSpots(StateController controller)
        {
            float minDist = Mathf.Infinity;
            filterSpots = new Dictionary<float, Vector3>();

            int nextCoverHash = -1;
            for(int i = 0; i < allCoverSpots.Count; ++i)
            {
                if (!covers[i].activeSelf || coverHashCodes[i] == controller.coverHash)
                {
                    continue;
                }

                foreach(Vector3 spot in allCoverSpots[i])
                {
                    Vector3 vectorDist = controller.personalTarget - spot;

                    float searchDist = (controller.transform.position - spot).sqrMagnitude;

                    if(vectorDist.sqrMagnitude <= controller.viewRadius * controller.viewRadius &&
                        Physics.Raycast(spot, vectorDist, out RaycastHit hit, vectorDist.sqrMagnitude,
                        controller.generalStats.coverMask))
                    {
                        // 플레이어가 Npc와 Spot사이에 있지 않은 지 확인 하고(보이는 각도의 1/4각도 만큼을 사용)
                        // -> 타켓보다 멀리 있으면 생략하기 위함
                        if(hit.collider == covers[i].GetComponent<Collider>() &&
                            !TargetInPath(controller.transform.position, spot, controller.personalTarget,
                            controller.viewAngle / 4))
                        {
                            if (!filterSpots.ContainsKey(searchDist))
                            {
                                filterSpots.Add(searchDist, spot);
                            }
                            else
                            {
                                continue;
                            }

                            if( minDist > searchDist)
                            {
                                minDist = searchDist;
                                nextCoverHash = coverHashCodes[i];
                            }
                        }
                    }
                }
            }
            ArrayList returnArray = new ArrayList();
            returnArray.Add(nextCoverHash);
            returnArray.Add(minDist);
            return returnArray;
        }

        public ArrayList GetBestCoverSpot(StateController controller)
        {
            ArrayList nextCoverData = FilterSpots(controller);
            int nextCoverHash = (int)nextCoverData[0];
            float minDist = (float)nextCoverData[1];

            ArrayList returnArray = new ArrayList();

            if(filterSpots.Count == 0)
            {
                // 찾지 못했다면..
                returnArray.Add(-1);
                returnArray.Add(Vector3.positiveInfinity);
            }
            else
            {
                returnArray.Add(nextCoverHash);
                returnArray.Add(filterSpots[minDist]);
            }

            return returnArray;
        }

    }

}