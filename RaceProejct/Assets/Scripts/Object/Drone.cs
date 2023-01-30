using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Drone : MonoBehaviour
{
    public IObjectPool<Drone> Pool { get; set; }

    public float _currentHelath;

    [SerializeField]
    private float maxHelath = 100.0f;

    [SerializeField]
    private float timeToSelfDestruct = 3.0f;

    void start()
    {
        _currentHelath = maxHelath;
    }

    private void OnEnable()
    {
        AttackPlayer();
        StartCoroutine(SelfDestruct());
    }

    void OnDisable()
    {
        // 풀로 되돌리기 전에, 초기 상태로 되돌리도록 함.
        ResetDrone();
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(timeToSelfDestruct);
        TakeDamage(maxHelath);
    }

    private void ReturnToPool()
    {
        Pool.Release(this);
    }

    void ResetDrone()
    {
        _currentHelath = maxHelath;
    }

    public void AttackPlayer()
    {
        DebugWrapper.Log("Attakc Player");
    }

    public void TakeDamage(float amount)
    {
        _currentHelath -= amount;

        if(_currentHelath <= 0.0f)
        {
            ReturnToPool();
        }
    }
}
