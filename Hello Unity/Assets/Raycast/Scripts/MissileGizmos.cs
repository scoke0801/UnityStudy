using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileGizmos : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue; // 기즈모 색상, 파랑색으로 지정
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
